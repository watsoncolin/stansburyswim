using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FillThePool.Models;

namespace FillThePool.Web.Filters
{
	public class NexmoActionFilter : ActionFilterAttribute, IActionFilter
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			
		}
	}
}