using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Elmah;

namespace FillThePool.Web.Controllers.API
{
    public class ErrorController : ApiController
    {
		public HttpResponseMessage Post(ErrorInputModel errorMessage)
		{
			var ex = new ClientErrorException(errorMessage.ToString());

			ErrorSignal signal = ErrorSignal.FromCurrentContext();
			signal.Raise(ex);

			return Request.CreateResponse(HttpStatusCode.OK, "{success:true}");
		}
    }

	public class ErrorInputModel
	{
		private string ErrorText { get; set; }
		private string Url { get; set; }
		private int? LineNumber { get; set; }

		public override string ToString()
		{
			return string.Format("ErrorText: {0}, URL: {1}, LineNumber: {2}", 
				ErrorText, 
				Url, 
				(LineNumber.HasValue ? LineNumber.Value.ToString() : "unknown"));
		}
	}

	public class ClientErrorException : Exception
	{
		public ClientErrorException(string message) : base(message) { }
	}
}
