using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FillThePool.Data;
using FillThePool.Models;
using FillThePool.Web.Filters;
using FillThePool.Web.ViewModels;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers
{
    public class LessonController : Controller
    {

		private readonly ApplicationUnit _unit = new ApplicationUnit();
        //
        // GET: /Lessons/

        [Authorize]
        public ActionResult Index()
        {
			var vm = new ScheduleViewModel();
			
            return View(vm);
        }

        [Authorize]
        public ActionResult Upcoming()
        {
	        var startTime = DateTime.Now.AddHours(-7);
            List<Schedule> model;
	        if (!User.IsInRole("Admin"))
		        model =
			        _unit.Schedules.GetAll()
						.Where(x => x.Registration.Student.User.UserId == WebSecurity.CurrentUserId && x.Start > startTime)
				        .OrderBy(x => x.Start)
				        .ToList();
	        else
		        model =
			        _unit.Schedules.GetAll()
						.Where(x => x.Start > startTime && x.Registration != null)
				        .OrderBy(x => x.Start)
				        .Take(15)
				        .ToList();

	        return PartialView("_Upcoming", model);
        }

        [Authorize]
        public ActionResult List(DateTime start)
        {
	        var firstOrDefault = _unit.Schedules.GetAll().OrderBy(s => s.Start).FirstOrDefault(s => s.Start > DateTime.Now && s.Registration == null);
	        if (firstOrDefault != null)
		        ViewBag.NextDate = firstOrDefault.Start.ToShortDateString();

	        var now = DateTime.Now.AddHours(-7);

	        var model = new UserRegistrationViewModel();
	        model.Schedules =
		        _unit.Schedules.GetAll()
			        .Where(
				        x =>
					        x.Start.Year == start.Year && x.Start.Month == start.Month && x.Start.Day == start.Day &&
					        x.Start > now)
			        .OrderBy(x => x.Start)
			        .ToList();
	        model.Students = Roles.IsUserInRole("Admin") ? _unit.Students.GetAll().ToList() : _unit.Students.GetAll().Where(x => x.User.UserId == WebSecurity.CurrentUserId).ToList();
	        
			return PartialView("_Schedule", model);
        }

		protected override void Dispose(bool disposing)
		{
			_unit.Dispose();
			base.Dispose(disposing);
		}
    }
}
