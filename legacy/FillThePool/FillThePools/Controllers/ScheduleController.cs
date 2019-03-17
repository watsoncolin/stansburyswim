using FillThePool.Data;
using FillThePool.Web.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace FillThePool.Web.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ApplicationUnit _unit = new ApplicationUnit();
        //
        // GET: /Schedule/
        [AllowAnonymous]
        public ActionResult Index()
        {
            var query = _unit.Schedules.GetAll().OrderByDescending(s => s.Start);
			var vw = new ScheduleListViewModel {Schedules = query.ToList()};

	        return View("Index", vw);
        }

        protected override void Dispose (bool disposing)
        {
            _unit.Dispose();
            base.Dispose(disposing);
        }
	}
}