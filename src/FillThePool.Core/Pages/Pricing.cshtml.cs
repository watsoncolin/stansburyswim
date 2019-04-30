using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		public PricingModel(IOptions<PayPalOptions> paypalOptions, UserManager<IdentityUser> userManager, ProfileService profileService)
		{
			_paypalOptions = paypalOptions.Value;
			_userManager = userManager;
			_profileService = profileService;
		}

		public string PayPalClientId => _paypalOptions.PayPalClientId;
		public bool IsEmailConfirmed { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user != null && !await _profileService.IsComplete(user))
			{
				return RedirectToPage("./Account/Manage/Index", new { area = "Identity" });
			}

			return Page();
		}
	}
}