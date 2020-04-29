using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FillThePool.Core.Pages
{
	[Authorize]
    public class ScheduleModel : PageModel
    {
		PayPalOptions _paypalOptions;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ProfileService _profileService;
		public ScheduleModel(UserManager<IdentityUser> userManager, IOptions<PayPalOptions> paypalOptions,ProfileService profileService)
		{
			_userManager = userManager;
			_paypalOptions = paypalOptions.Value;
			_profileService = profileService;
		}

		public string PayPalClientId => _paypalOptions.PayPalClientId;

        public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user != null)
			{
				var isProfileComplete = await _profileService.IsProfileComplete(user);
				if (!isProfileComplete)
				{
					return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
				}
			}
			return Page();
		}
    }
}