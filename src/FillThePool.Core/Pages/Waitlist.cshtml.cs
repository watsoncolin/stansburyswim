using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Pages
{
    public class WaitlistModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _context;
		public WaitlistModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[BindProperty]
		[DisplayName("Join Wait list")]
		public bool OnWaitList { get; set; }
		public async Task OnGet()
		{
			var user = await _userManager.GetUserAsync(User);
			var profile = _context.Profiles.First(p => p.IdentityUserId == user.Id);
			var waitlist = _context.Waitlist.FirstOrDefault(w => w.ProfileId == profile.Id);

			OnWaitList = waitlist != null;
		}

		public async Task<IActionResult> OnPostSaveWaitlistAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			var profile = _context.Profiles.First(p => p.IdentityUserId == user.Id);
			var waitlist = _context.Waitlist.FirstOrDefault(w => w.ProfileId == profile.Id);

			if (!OnWaitList && waitlist != null)
			{
				_context.Waitlist.Remove(waitlist);
			}

			if (OnWaitList)
			{
				_context.Waitlist.Add(new Waitlist
				{
					ProfileId = profile.Id,
					DateJoined = DateTime.UtcNow
				});
			}

			await _context.SaveChangesAsync();

			return RedirectToPage("./Waitlist");
		}
	}
}