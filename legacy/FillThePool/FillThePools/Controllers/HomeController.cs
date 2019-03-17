using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FillThePool.Data;
using FillThePool.Models;
using FillThePool.Web.Filters;
using FillThePool.Web.ViewModels;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers
{
	[Authorize]
	[InitializeSimpleMembership] 
    public class HomeController : Controller
    {
        private readonly ApplicationUnit _unit = new ApplicationUnit();

        public ActionResult PayPalComplete(string token, string payerId)
        {
	        var home = new HomeViewModel
	        {
		        Students = _unit.Students.GetAll().Where(x => x.User.UserId == WebSecurity.CurrentUserId).ToList()
	        };

            PayPalTransaction.CompleteTransaction(token, payerId);
            ViewBag.Tour = true;
            ViewBag.PayPalComplete = true;
            return View("Index", home);
        }
		[AllowAnonymous]
        public ActionResult Index(bool? tour)
		{

			//var util = new Utilities();
			//util.SetupLessons();
			



			var home = new HomeViewModel
			{
				Students = _unit.Students.GetAll().Where(x => x.User.UserId == WebSecurity.CurrentUserId).ToList()
			};

			ViewBag.Message =  "Welcome! Let's Get Our Feet wet.";

            if (tour != null && tour == true)
                ViewBag.Tour = true;
            else
                ViewBag.Tour = false;

            return View(home);
        }

        public ActionResult Students()
        {
            return PartialView("_Students", _unit.Students.GetAll().Where(x => x.User.UserId == WebSecurity.CurrentUserId).AsEnumerable());            
        }

        public ActionResult Transaction()
        {
            return PartialView("_Transaction");
        }


		[AllowAnonymous]
		public ActionResult LoginForm()
		{
			var model = new LoginModel();

			return PartialView("_LoginForm", model);
		}

		[AllowAnonymous]
        public ActionResult About()
		{
			var page = new PageViewModel {Page = _unit.Pages.GetAll().FirstOrDefault(x => x.Title == "About")};

            ViewBag.Message = "Your app description page.";

            return View(page);
        }

		[AllowAnonymous]
		public ActionResult Instructors()
		{
			var page = _unit.Pages.GetAll().FirstOrDefault(x => x.Title == "Instructors");
			if (page == null)
			{
				_unit.Pages.Add(new Page
				{
					Title = "Instructors"
				});
				_unit.SaveChanges();
			}
			var pagevm = new PageViewModel { Page = _unit.Pages.GetAll().FirstOrDefault(x => x.Title == "Instructors") };
			
			return View(pagevm);
		}

		[AllowAnonymous]
        public ActionResult Privacy()
        {
            ViewBag.Message = "This is the privacy policy.";

            return View();
        }

		[AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


		protected override void Dispose(bool disposing)
		{
			_unit.Dispose();
			base.Dispose(disposing);
		}
    }
}
