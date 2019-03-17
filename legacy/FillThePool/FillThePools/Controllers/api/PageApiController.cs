using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FillThePool.Data;
using FillThePool.Models;

namespace FillThePool.Web.Controllers.API
{
    public class PageApiController : ApiController
    {

		private readonly ApplicationUnit _unit = new ApplicationUnit();

		[HttpPost]
		[Authorize(Roles = "Admin")]
	    public HttpResponseMessage Post(Page page)
		{
			var p = _unit.Pages.GetById(page.PageId);
			p.Html = page.Html;
			_unit.SaveChanges();

			return Request.CreateResponse(HttpStatusCode.Accepted, page.PageId);
		}
    }
}
