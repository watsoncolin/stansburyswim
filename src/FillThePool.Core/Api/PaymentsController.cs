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
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;

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
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ILogger<PaymentsController> _logger;
		private readonly PayPalOptions _paypalOptions;
		private readonly ApplicationDbContext _context;

		public PaymentsController(IOptions<PayPalOptions> paypalOptions, ILogger<PaymentsController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
			_context = context;
			_logger = logger;
			_paypalOptions = paypalOptions.Value;
		}

		[HttpGet]
		[Route("available-credits")]
		public async Task<IActionResult> AvailableCredits()
		{
			var user = await _userManager.GetUserAsync(User);
			var profile = _context.Profiles.FirstOrDefault(p => p.IdentityUserId == user.Id);
			var credits = _context.Transactions.Where(t => t.ProfileId == profile.Id).Sum(t => t.LessonCredit);
			return Ok(credits);
		}


		[HttpPost]
		[Route("verify-payment")]
		public async Task<IActionResult> VerifyPayment([FromBody]VerifyPayment payment)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

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
			var result = response.Result<Order>();

			_logger.LogInformation("Transaction details: {0}", JsonConvert.SerializeObject(new
			{
				status = result.Status,
				orderId = result.Id,
				intent = result.Intent,
				links = result.Links,
				purchaseUnits = result.PurchaseUnits
			}));


			AmountWithBreakdown amount = result.PurchaseUnits[0].Amount;

			var profile = _context.Profiles.FirstOrDefault(p => p.IdentityUserId == user.Id);

			if (profile == null)
			{
				_logger.LogInformation("No profile found for user id {0}", user.Id);
				return BadRequest();
			}

			switch (result.PurchaseUnits[0].CustomId)
			{
				case "single_lesson":
					{
						if (amount.Value == "16.00")
						{
							_context.Transactions.Add(new Transaction
							{
								ProfileId = profile.Id,
								Amount = decimal.Parse(amount.Value),
								Description = result.PurchaseUnits[0].Description,
								TimeStamp = DateTime.Parse(result.CreateTime),
								PayPalPaymentId = result.Id,
								PayPalPayerId = result.Payer.PayerId,
								LessonCredit = 1,
								Type = "Purchased",
							});

							await _context.SaveChangesAsync();
						}
						break;
					}
				case "ten_lesson":
					{
						if (amount.Value == "140.00")
						{
							_context.Transactions.Add(new Transaction
							{
								ProfileId = profile.Id,
								Amount = decimal.Parse(amount.Value),
								Description = result.PurchaseUnits[0].Description,
								TimeStamp = DateTime.Parse(result.CreateTime),
								PayPalPaymentId = result.Id,
								PayPalPayerId = result.Payer.PayerId,
								LessonCredit = 10,
								Type = "Purchased",
							});

							await _context.SaveChangesAsync();
						}
						break;
					}
				case "thirty_lesson":
					{
						if (amount.Value == "360.00")
						{
							_context.Transactions.Add(new Transaction
							{
								ProfileId = profile.Id,
								Amount = decimal.Parse(amount.Value),
								Description = result.PurchaseUnits[0].Description,
								TimeStamp = DateTime.Parse(result.CreateTime),
								PayPalPaymentId = result.Id,
								PayPalPayerId = result.Payer.PayerId,
								LessonCredit = 30,
								Type = "Purchased",
							});

							await _context.SaveChangesAsync();
						}
						break;
					}
				default:
					{
						_logger.LogInformation("Unable to complete transaction.  TransactionId: {0}", result.Id);
						return BadRequest();
					}
			}

			// Send email


			return Ok();
		}
	}
}