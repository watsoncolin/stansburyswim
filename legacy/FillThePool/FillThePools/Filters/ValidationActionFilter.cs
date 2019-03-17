using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace FillThePool.Web.Filters
{
    public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
	        if (modelState.IsValid) return;
	        var errors = new JObject();
	        foreach (var key in modelState.Keys)
	        {
		        var state = modelState[key];
		        if (state.Errors.Any())
		        {
			        errors[key] = state.Errors.First().ErrorMessage;
		        }
	        }

	        actionContext.Response = actionContext.Request.
		        CreateResponse(HttpStatusCode.BadRequest, errors);
        }
    }
}