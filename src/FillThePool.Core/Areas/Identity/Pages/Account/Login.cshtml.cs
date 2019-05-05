using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FillThePool.Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
	{
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ProfileService _profileService;
		private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ProfileService profileService, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
			_userManager = userManager;
			_profileService = profileService;
			_logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Schedule");

            if (ModelState.IsValid)
            {
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var user = await  _userManager.FindByIdAsync("bfe0d2a5-aa03-4449-b826-51ea914e5f51");
				await _signInManager.SignInAsync(user, false);


                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
				var user1 = await _userManager.FindByNameAsync(Input.Email);
				return RedirectToPage("/Account/Manage/Index", new { Area = "Identity" });

				if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

					if(!await _profileService.IsComplete(user))
					{
						return RedirectToPage("/Account/Manage/Index", new { Area = "Identity" });
					}

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
				if(result.IsNotAllowed)
				{
					var confirmed = await _userManager.IsEmailConfirmedAsync(user);
					if (!confirmed)
					{
						_logger.LogWarning("User email not confirmed.");
					}

					ModelState.AddModelError(string.Empty, "Please confirm your email address.");
					ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
					return Page();
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
				return Page();                
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
