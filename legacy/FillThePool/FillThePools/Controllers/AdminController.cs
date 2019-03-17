using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FillThePool.Data;
using FillThePool.Models;
using FillThePool.Web.ViewModels;

namespace FillThePool.Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private const string WaitListSetting = "WaitList";
		private readonly ApplicationUnit _unit = new ApplicationUnit();
		//
		// GET: /Admin/
		public ActionResult Index()
		{

			var model = new AdminUserListViewModel();
			var users = _unit.Users.GetAll().OrderBy(u => u.LastName);
			var waitlist = _unit.Settings.GetAll().FirstOrDefault(s => s.Key == WaitListSetting);
			if (waitlist != null && waitlist.Value == "true")
			{
				model.WaitListEnabled = true;
			}
			else
			{
				model.WaitListEnabled = false;
			}

			foreach (var u in users)
			{
				var uservm = new AdminUserViewModel();
				uservm.User = u;
				uservm.Students = _unit.Students.GetAll().Where(s => s.User.UserId == u.UserId).ToList();
				uservm.Transactions = _unit.Transactions.GetAll().Where(t => t.User.UserId == u.UserId).ToList();
				uservm.Balance = Helpers.CheckBalance(u.UserId);

				model.Users.Add(uservm);
			}

			ViewBag.TotalCreditsPurchased = _unit.Transactions.GetAll().Where(t => !t.Description.Contains("PENDING")).Sum(t => t.LessonCredit);
			ViewBag.TotalLessonsReserved = _unit.Schedules.GetAll().Count(s => s.Registration != null);


			return View(model);
		}

		public PartialViewResult Edit(int id)
		{
			var user = _unit.Users.GetById(id);
			var roleType = RoleTypes.User;
			if (Roles.IsUserInRole(user.UserName, "Admin"))
				roleType = RoleTypes.Admin;
			else if (Roles.IsUserInRole(user.UserName, "Instructor"))
				roleType = RoleTypes.Instructor;

			var model = new AdminUserEditViewModel
			{
				User = user,
				Role = roleType
			};


			return PartialView("_Edit", model);
		}

		[HttpPost]
		public PartialViewResult Edit(AdminUserEditViewModel model)
		{
			_unit.Users.Update(model.User);
			_unit.SaveChanges();

			try
			{
				Roles.RemoveUserFromRole(model.User.UserName, "Admin");
			}
			catch { }
			try
			{
				Roles.RemoveUserFromRole(model.User.UserName, "Instructor");
			}
			catch { }
			
			if (model.Role != RoleTypes.User)
				Roles.AddUserToRole(model.User.UserName, model.Role.ToString());

			return PartialView("_Edit", model);
		}

		public PartialViewResult Transactions(int id)
		{
			var transactions = _unit.Transactions.GetAll().Where(t => t.User.UserId == id).ToList().Select(transaction => new TransactionViewModel
			{
				Amount = transaction.Amount, Description = transaction.Description, LessonCredit = transaction.LessonCredit, Type = transaction.Type, UserId = id, CreatedOn = transaction.CreatedOn
			}).ToList();

			ViewBag.UserId = id;

			return PartialView("_Transactions", transactions);
		}

		[HttpPost]
		public PartialViewResult Transactions(TransactionViewModel model)
		{
			var userId = model.UserId;
			var transaction = new Transaction
			{
				Amount = model.Amount,
				Description = model.Type + " " + model.LessonCredit + " credits",
				LessonCredit = model.LessonCredit,
				Type = model.Type,
				User = _unit.Users.GetById(userId),
				CreatedOn = DateTime.Now,
				ModifiedOn = DateTime.Now,
				TimeStamp = DateTime.Now
			};

			_unit.Transactions.Add(transaction);
			_unit.SaveChanges();
			var viewModel = _unit.Transactions.GetAll().Where(t => t.User.UserId == userId).ToList().Select(tran => new TransactionViewModel
			{
				Amount = tran.Amount, Description = tran.Description, LessonCredit = tran.LessonCredit, Type = tran.Type, UserId = userId, CreatedOn = tran.CreatedOn
			}).ToList();

			ViewBag.UserId = userId;
			
			return PartialView("_Transactions", viewModel);
		}

		public PartialViewResult Lessons(int id)
		{
			var schedules = _unit.Schedules.GetAll().Where(s => s.Registration.Student.User.UserId == id && s.Start > DateTime.Now).OrderBy(s => s.Start);

			return PartialView("_Lessons", schedules);
		}

		public PartialViewResult Summary(int id)
		{
			var model = new AdminUserViewModel
			{
				User = _unit.Users.GetById(id),
				Students = _unit.Students.GetAll().Where(s => s.User.UserId == id).ToList(),
				Transactions = _unit.Transactions.GetAll().Where(t => t.User.UserId == id).ToList(),
				Balance = Helpers.CheckBalance(id)
			};

			return PartialView("_Summary", model);
		}
		[OverrideAuthorization]
		[Authorize(Roles = "Admin, Instructor")]
	    public ActionResult Schedule(DateTime? date)
	    {
	        List<Schedule> model;

	        if (date != null)
	        {
                model = _unit.Schedules.GetAll().Where(s => s.Start.Year == date.Value.Year && s.Start.Month == date.Value.Month && s.Start.Day == date.Value.Day).ToList();
	        }
	        else
	        {
                model = _unit.Schedules.GetAll().Where(s => s.Start.Year == DateTime.Now.Year &&
                    s.Start.Month == DateTime.Now.Month &&
                    s.Start.Day == DateTime.Now.Day).ToList();
	        }

	        return View(model);
	    }

		public ActionResult WaitList()
		{
			var model = _unit.WaitList.GetAll().Select(w => new AdminWaitListViewModel {WaitList = w}).OrderBy(w => w.WaitList.DateAdded).ToList();
			return View(model);
		}

		public ActionResult ToggleWaitList()
		{
			var setting =_unit.Settings.GetAll().FirstOrDefault(s => s.Key == WaitListSetting);
			if (setting == null)
			{
				setting = new Setting
				{
					Key = WaitListSetting,
					Value = "true",
					CreatedOn = DateTime.Now,
					ModifiedOn = DateTime.Now
				};
				_unit.Settings.Add(setting);
				_unit.SaveChanges();
			}
			else
			{
				setting.Value = setting.Value == "true" ? "false" : "true";
				_unit.SaveChanges();
			}


			return RedirectToAction("Index");
		}

		public ActionResult Export()
		{
			string result = "FirstName,LastName,Email,Balance,HasPurchased \n";
			foreach (var u in _unit.Users.GetAll())
			{
				result += string.Format("{0},{1},{2},{3},{4}\n", u.FirstName, u.LastName, u.Email,
					Helpers.CheckBalance(u.UserId), Helpers.HasPurchased(u.UserId));
			}


			Response.Clear();
			Response.AddHeader("Content-Disposition", "attachment; filename=users.csv");
			Response.ContentType = "text/csv";
			Response.Write(result);
			Response.End();

			return RedirectToAction("Index");
		}


		protected override void Dispose(bool disposing)
		{
			_unit.Dispose();
			base.Dispose(disposing);
		}

	}
}