using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FillThePool.Data;
using FillThePool.Models;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers.API
{
	[Authorize]
    public class UserApiController : ApiController
	{
		private readonly ApplicationUnit _unit = new ApplicationUnit();
		// PUT api/Schedule/5
		[Authorize]
		public HttpResponseMessage PutUser(int id, User user)
		{
			if (!ModelState.IsValid)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
			}

			if (id != user.UserId)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			if (user.UserId == WebSecurity.CurrentUserId)
			{
				User existing = _unit.Users.GetById(user.UserId);
				_unit.Users.Detatch(existing);
				user.CreatedOn = existing.CreatedOn;

				_unit.Users.Update(user);

				try
				{
					_unit.SaveChanges();

					return Request.CreateResponse(HttpStatusCode.OK, "{success: 'true', verb: 'PUT'}");
				}
				catch (DbUpdateConcurrencyException ex)
				{
					return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
				}
			}
			return Request.CreateResponse(HttpStatusCode.BadRequest);
		}


		protected override void Dispose(bool disposing)
		{
			_unit.Dispose();
			base.Dispose(disposing);
		}
    }
}
