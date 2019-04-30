using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FillThePool.Core.Areas.Identity.Pages.Account.Manage
{
	public partial class IndexModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly ApplicationDbContext _context;
		private readonly ILogger<IndexModel> _logger;
		private readonly ProfileService _profileService;

		public IndexModel(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			IEmailSender emailSender,
			ApplicationDbContext context,
			ProfileService profileService,
			ILogger<IndexModel> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_context = context;
			_logger = logger;
			_profileService = profileService;
		}

		public bool IsEmailConfirmed { get; set; }
		public bool IsProfileSetup { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			public string Username { get; set; }
			[Required]
			[EmailAddress]
			public string Email { get; set; }
			[Phone]
			[Required]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }
			[Required]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }
			[Required]
			[Display(Name = "Last Name")]
			public string LastName { get; set; }
			[Display(Name = "How did you hear about us?")]
			public string Referral { get; set; }
			[Required]
			[Display(Name = "Address")]
			public string Address1 { get; set; }
			[Display(Name = "Line 2")]
			public string Address2 { get; set; }
			[Required]
			[Display(Name = "City")]
			public string City { get; set; }
			[Required]
			[Display(Name = "State")]
			public string State { get; set; }
			[Required]
			[Display(Name = "Zip")]
			public string Zip { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var userName = await _userManager.GetUserNameAsync(user);
			var email = await _userManager.GetEmailAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);
			if (profile == null)
			{
				_logger.LogInformation("No existing profile found.");
				profile = new Profile
				{
					IdentityUserId = user.Id
				};
			}


			Input = new InputModel
			{
				Username = userName,
				Email = email,
				FirstName = profile.FirstName,
				LastName = profile.LastName,
				PhoneNumber = phoneNumber,
				Address1 = profile.Address1,
				Address2 = profile.Address2,
				City = profile.City,
				State = profile.State,
				Zip = profile.Zip,
				Referral = profile.Referral,
			};

			IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
			IsProfileSetup = await _profileService.IsProfileComplete(user);
			var hasStudents = await _profileService.HasStudent(user);

			if (IsProfileSetup && !hasStudents)
			{
				return RedirectToPage("./Students");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (Input.Username != user.UserName)
			{
				var setUsernameResult = await _userManager.SetUserNameAsync(user, Input.Username);
				if (!setUsernameResult.Succeeded)
				{
					foreach (var error in setUsernameResult.Errors)
					{
						_logger.LogInformation("User failed to update username.");
						ModelState.AddModelError("Username", error.Description);
						return Page();
					}
				}
			}

			var email = await _userManager.GetEmailAsync(user);
			if (Input.Email != email)
			{
				var existingUser = await _userManager.FindByEmailAsync(Input.Email);
				if (existingUser != null)
				{
					_logger.LogInformation("User failed to update email due to email already existing in the system.");
					ModelState.AddModelError("Email", "The specified email address is already in use.");
					return Page();
				}

				var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
				if (!setEmailResult.Succeeded)
				{
					var userId = await _userManager.GetUserIdAsync(user);
					throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
				}
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber)
			{
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded)
				{
					var userId = await _userManager.GetUserIdAsync(user);
					throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
				}
			}

			var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);
			if (profile == null)
			{
				profile = new Profile { IdentityUserId = user.Id };
				_logger.LogInformation("Creating a new profile.");
			}
			else
			{
				_logger.LogInformation("Updating user profile.");
			}

			profile.FirstName = Input.FirstName;
			profile.LastName = Input.LastName;
			profile.Address1 = Input.Address1;
			profile.Address2 = Input.Address2;
			profile.City = Input.City;
			profile.State = Input.State;
			profile.Zip = Input.Zip;
			profile.Referral = Input.Referral;
			profile.Phone = Input.PhoneNumber;

			if (profile.Id == 0)
				_context.Profiles.Add(profile);

			await _context.SaveChangesAsync();

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostSendVerificationEmailAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}


			var userId = await _userManager.GetUserIdAsync(user);
			var email = await _userManager.GetEmailAsync(user);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { userId = userId, code = code },
				protocol: Request.Scheme);
			await _emailSender.SendEmailAsync(
				email,
				"Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

			StatusMessage = "Verification email sent. Please check your email.";
			return RedirectToPage();
		}
	}
}
