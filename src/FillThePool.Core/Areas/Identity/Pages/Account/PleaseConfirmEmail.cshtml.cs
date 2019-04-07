using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class PleaseConfirmEmailModel : PageModel
    {
        public PleaseConfirmEmailModel()
        {
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            return Page();
        }
    }
}
