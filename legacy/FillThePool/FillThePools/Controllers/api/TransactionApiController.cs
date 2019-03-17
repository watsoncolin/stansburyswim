using System;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Web.Http;
using FillThePool.Data;
using WebMatrix.WebData;

namespace FillThePool.Web.Controllers.API
{
    [Authorize]
    public class TransactionApiController : ApiController
	{
		private readonly ApplicationUnit _unit = new ApplicationUnit();

		[HttpGet]
		[Authorize]
        public int Balance()
        {
		    return Helpers.CheckBalance(WebSecurity.CurrentUserId);
        }

		// POST api/Transaction
        [HttpPost]
        [Authorize]
		public string CreatePayPalTransaction(TransactionDto transactionDto)
		{
			var credit = 0;
			//check if we have a valid promo code
			var code = _unit.PromoCodes.GetAll().FirstOrDefault(x => x.Code.ToLower() == transactionDto.PromoCode.ToLower());

			if (code != null && code.Code == "splash")
			{
				switch (Int32.Parse(Math.Round(transactionDto.Amount).ToString(CultureInfo.InvariantCulture)))
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
			else
			{
				switch (Int32.Parse(Math.Round(transactionDto.Amount).ToString(CultureInfo.InvariantCulture)))
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

	        var description = "Purchased " + credit + " Credits";

	        if (credit > 0)
				return PayPalTransaction.StartTransaction(transactionDto.Amount.ToString(), description,
			        "HTTP://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Transaction/",
			        "http://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Home/PayPalComplete", credit);
	        return "";
		}


		protected override void Dispose(bool disposing)
		{
			_unit.Dispose();
			base.Dispose(disposing);
		}
    }

	public class TransactionDto
	{
		public decimal Amount { get; set; }
		public string PromoCode { get; set; }
	}
}
