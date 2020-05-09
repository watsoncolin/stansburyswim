using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailErrorModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ConfirmEmailErrorModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public string UserId { get; set; }

        public IActionResult OnGet(string userId)
        {
            UserId = userId;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            await RegisterModel.SendEmailConfirmation(_userManager, _emailSender, Url, user);

            return RedirectToPage("PleaseConfirmEmail");
        }
    }
}
