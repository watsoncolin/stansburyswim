using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
	public class EditModel : PageModel
    {
        public IActionResult OnGet()
        {
			if(User.Claims.Any(c => c.Type == "Admin" && c.Value != "Full"))
            {
                return new RedirectToPageResult("/Manage/PrintSchedule");
			}

            return Page();
        }
    }
}