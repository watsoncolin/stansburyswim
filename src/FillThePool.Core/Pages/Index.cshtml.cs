using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FillThePool.Core.Pages
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}
		public async Task OnGet()
		{
		}
	}
}
