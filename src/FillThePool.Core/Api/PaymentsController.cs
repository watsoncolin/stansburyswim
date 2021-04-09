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
		private readonly EmailService _emailService;

		public PaymentsController(IOptions<PayPalOptions> paypalOptions, ILogger<PaymentsController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, EmailService emailService)
		{
			_userManager = userManager;
			_context = context;
			_logger = logger;
			_paypalOptions = paypalOptions.Value;
			_emailService = emailService;
		}

		[HttpGet]
		[Route("available-credits")]
		public async Task<IActionResult> AvailableCredits()
		{
			var user = await _userManager.GetUserAsync(User);
			var credits = Utilities.GetAvailableCredits(_context, user.Id);
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
			var profile = _context.Profiles.FirstOrDefault(p => p.IdentityUserId == user.Id);

			var settings = _context.Settings.FirstOrDefault();
			if (settings != null)
			{
				if (settings.WaitlistEnabled)
				{
					var userWaitlist = _context.Waitlist.FirstOrDefault(w => w.ProfileId == profile.Id);
					if (userWaitlist == null || !userWaitlist.AllowedPurchase)
					{
						return RedirectToPage("/waitlist");
					}
				}
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


			if (profile == null)
			{
				_logger.LogInformation("No profile found for user id {0}", user.Id);
				return BadRequest();
			}

			switch (result.PurchaseUnits[0].CustomId)
			{
				case "single_lesson":
					{
						if (amount.Value == "20.00")
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
						if (amount.Value == "170.00")
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
						if (amount.Value == "450.00")
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
			await _emailService.SendPurchaseEmail(user);


			// Remove user from waitlist if the user is on it
			var waitlist = _context.Waitlist.FirstOrDefault(w => w.ProfileId == profile.Id);
			if (waitlist != null)
			{
				_context.Waitlist.Remove(waitlist);
				await _context.SaveChangesAsync();
			}

			return Ok();
		}
	}
}