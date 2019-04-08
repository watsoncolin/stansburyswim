using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BraintreeHttp;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace FillThePool.Core.Api
{
	public class VerifyPayment
	{
		public string OrderId { get; set; }
	}
	[Route("api/payments")]
	[ApiController]
	public class PaymentsController : ControllerBase
	{
		private readonly ILogger<PaymentsController> _logger;
		private readonly PayPalOptions _paypalOptions;

		public PaymentsController(IOptions<PayPalOptions> paypalOptions, ILogger<PaymentsController> logger)
		{
			_logger = logger;
			_paypalOptions = paypalOptions.Value;
		}

		[HttpPost]
		[Route("verify-payment")]
		public async Task<IActionResult> VerifyPayment([FromBody]VerifyPayment payment)
		{
			OrdersGetRequest request = new OrdersGetRequest(payment.OrderId);

			PayPalEnvironment environment;

			if (_paypalOptions.PayPalMode == "production")
			{
				environment = new LiveEnvironment(_paypalOptions.PayPalClientId, _paypalOptions.PayPalSecret);
				_logger.LogInformation("Using paypal production environment");
			}
			else
			{
				environment = new SandboxEnvironment(_paypalOptions.PayPalClientId, _paypalOptions.PayPalSecret);
				_logger.LogInformation("Using paypal sandbox environment");
			}

			_logger.LogInformation("Verifying paypal payment", new
			{
				payment.OrderId
			});

			var client = new PayPalHttpClient(environment);

			var response = await client.Execute(request);
			//4. Save the transaction in your database. Implement logic to save transaction to your database for future reference.
			var result = response.Result<Order>();

			_logger.LogInformation("Transaction details: {0}", JsonConvert.SerializeObject(new
			{
				status = result.Status,
				orderId = result.Id,
				intent = result.Intent,
				links = result.Links,
				purchaseUnits = result.PurchaseUnits
			}));
			
			Console.WriteLine("Retrieved Order Status");
			Console.WriteLine("Status: {0}", result.Status);
			Console.WriteLine("Order Id: {0}", result.Id);
			Console.WriteLine("Intent: {0}", result.Intent);
			Console.WriteLine("Links:");
			foreach (LinkDescription link in result.Links)
			{
				Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
			}
			AmountWithBreakdown amount = result.PurchaseUnits[0].Amount;
			Console.WriteLine("Total Amount: {0} {1}", amount.CurrencyCode, amount.Value);
			return Ok(response);
		}
	}
}