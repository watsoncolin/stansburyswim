using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using FillThePool.Data;
using FillThePool.Web.Filters;
using FillThePool.Web.ViewModels;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers
{
	public class TransactionController : Controller
	{
		private readonly ApplicationUnit _unit = new ApplicationUnit();

		//
		// GET: /Transaction/
		public ActionResult Index()
		{
			var waitlist = _unit.Settings.GetAll().FirstOrDefault(s => s.Key == "WaitList");
			if (!WebSecurity.IsAuthenticated)
				return View("PriceList");

			if (waitlist != null && waitlist.Value == "true")
				return RedirectToAction("WaitList");
			
			return View("Index");
		}


		public ActionResult WaitList()
		{
			var model = new WaitListViewModel();
			return string.IsNullOrEmpty(model.Code) ? View(model) : View("Index");
		}

		[HttpPost]
		public ActionResult WaitList(WaitListViewModel model)
		{
			var waitList =
				_unit.WaitList.GetAll()
					.FirstOrDefault(w => w.User.UserName == WebSecurity.CurrentUserName && w.PurchaseCode == model.Code);
			if (waitList != null)
				return View("Index");

			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(TransactionViewModel model)
		{
			var credit = 0;
			//check if we have a valid promo code
			var code = _unit.PromoCodes.GetAll().FirstOrDefault(x => x.Code.ToLower() == model.PromoCode.ToLower());

			//if (code != null && code.Code == "splash")
			//{
			//	switch (Int32.Parse(Math.Round(model.Amount).ToString(CultureInfo.InvariantCulture)))
			//	{
			//		case 10:
			//			credit = 1;
			//			break;
			//		case 100:
			//			credit = 11;
			//			break;
			//		case 200:
			//			credit = 23;
			//			break;
			//	}
			//}
			//else
			{
				switch (Int32.Parse(Math.Round(model.Amount).ToString(CultureInfo.InvariantCulture)))
				{
					case 15:
						credit = 1;
						break;
					case 130:
						credit = 10;
						break;
					case 300:
						credit = 30;
						break;
				}

			}

			if (credit <= 0) return View();
			if (Request.Url != null)
				Response.Redirect(PayPalTransaction.StartTransaction(model.Amount.ToString(CultureInfo.InvariantCulture),
					"Purchased " + credit + " Credits",
					Request.Url.ToString(), Request.Url + "/Complete", credit));

			return View("Index");
		}


		[Authorize]
		public ActionResult Complete()
		{
			string token = HttpContext.Request.QueryString["token"];
			string payerId = HttpContext.Request.QueryString["PayerID"];
			PayPalTransaction.CompleteTransaction(token, payerId);

			//clear user from waitlist
			var waitList = _unit.WaitList.GetAll().Where(w => w.User.UserName == WebSecurity.CurrentUserName);
			foreach (var w in waitList)
			{
				_unit.WaitList.Delete(w);
			}
			_unit.SaveChanges();

			Response.Redirect("/Transaction");

			return View("Index");
		}

		//
		// GET: /Transaction/Details/5

		[Authorize]
		public ActionResult History()
		{
			return PartialView("_TransactionHistory",
				_unit.Transactions.GetAll()
				.Where(x => x.User.UserId == WebSecurity.CurrentUserId && !x.Description.Contains("PENDING") && x.CreatedOn > new DateTime(2016, 5, 1))
				.OrderByDescending(x => x.TimeStamp)
				.ToList());
		}


		protected override void Dispose(bool disposing)
		{
			_unit.Dispose();
			base.Dispose(disposing);
		}
	}
}