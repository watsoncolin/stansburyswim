using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using FillThePool.Data;
using FillThePool.Models;
using WebMatrix.WebData;
using FillThePool.Web;

namespace FillThePool.Web.Controllers.API
{
    public class WaitListAPIController : ApiController
    {

		[Authorize]
		[HttpGet]
	    public HttpResponseMessage Update(bool onWaitList)
		{
			var unit = new ApplicationUnit();

			var user = unit.Users.GetAll().FirstOrDefault(u => u.UserName == WebSecurity.CurrentUserName);
			var waitList = unit.WaitList.GetAll().FirstOrDefault(u => u.User.UserName == WebSecurity.CurrentUserName);
			if (onWaitList)
			{
				if (waitList == null)
				{
					unit.WaitList.Add(new WaitList
					{
						User = user,
						DateAdded = DateTime.Now
					});
				}
			}
			else
			{
				if (waitList != null)
				{
					unit.WaitList.Delete(waitList);
				}
			}

			unit.SaveChanges();

			return Request.CreateResponse(HttpStatusCode.OK);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
	    public HttpResponseMessage Email(int id)
	    {
		    var unit = new ApplicationUnit();
		    var user = unit.Users.GetAll().FirstOrDefault(u => u.UserId == id);
		    var waitList = unit.WaitList.GetAll().FirstOrDefault(w => w.User.UserId == id);
			waitList.DateCodeAdded = DateTime.Now;

		    if (user != null)
		    {
			    waitList.PurchaseCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.UserName));
			    var message =string.Format("You can now purchase more credits at Stansbury Swim. This invitation is valid for 7 days.  You'll be able to make one transaction then you'll need to add yourself back to the wait list if you need more.  <a href='https://www.stansburyswim.com/transaction/waitlist/?code={0}'>Click here to purchase more credits.</a>", HttpUtility.UrlEncode(waitList.PurchaseCode));
			    Web.Email.SendEmail(user.Email, "Stansbury Swim Lessons Available", message);

				unit.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, waitList.PurchaseCode);
		    }
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
	    }


    }
}
