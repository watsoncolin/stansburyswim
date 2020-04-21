using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FillThePool.Core.Pages
{
	public class PricingModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly PayPalOptions _paypalOptions;
		private readonly ProfileService _profileService;
		private readonly ApplicationDbContext _context;
		public PricingModel(IOptions<PayPalOptions> paypalOptions, UserManager<IdentityUser> userManager, ProfileService profileService, ApplicationDbContext context)
		{
			_paypalOptions = paypalOptions.Value;
			_userManager = userManager;
			_profileService = profileService;
			_context = context;
		}

		public string PayPalClientId => _paypalOptions.PayPalClientId;
		public bool IsEmailConfirmed { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);

			var settings = _context.Settings.FirstOrDefault();
			if(settings != null)
			{
				if (settings.WaitlistEnabled && user == null)
				{
					return Page();
				}

				if (settings.WaitlistEnabled)
				{
					var profile = _context.Profiles.FirstOrDefault(p => p.IdentityUserId == user.Id);
					var waitlist = _context.Waitlist.FirstOrDefault(w => w.ProfileId == profile.Id);
					if (waitlist == null || (waitlist.AllowedPurchaseDate.AddDays(7) < DateTime.UtcNow))
					{
						return RedirectToPage("/waitlist");
					}
				}
			}

			if (user != null && !await _profileService.IsComplete(user))
			{
				return RedirectToPage("./Account/Manage/Index", new { area = "Identity" });
			}

			return Page();
		}
	}
}