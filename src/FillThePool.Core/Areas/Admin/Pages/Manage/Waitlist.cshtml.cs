using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FillThePool.Core.Areas.Admin.Pages
{
	public class WaitlistModel : PageModel
    {
		private readonly ApplicationDbContext _context;
		private readonly EmailService _emailService;
		public WaitlistModel(ApplicationDbContext context, EmailService emailService)
		{
			_context = context;
			_emailService = emailService;
		}
		[BindProperty]
		public SettingsModel Settings { get; set; } = new SettingsModel();
		[BindProperty]
		public List<WaitlistDisplayModel> Waitlist { get; set; } = new List<WaitlistDisplayModel>();
		public class WaitlistDisplayModel
		{
			public string Name { get; set; }
			public int ProfileId { get; set; }
			public DateTime DateJoined { get; set; }
			[DisplayName("Allowed Purchase")]
			public bool AllowedPurchase { get; set; }
			public bool Expired { get; set; }
		}
		public class SettingsModel
		{
			[DisplayName("Wait list Enabled")]
			public bool WaitlistEnabled { get; set; }
		}
		
		public async Task OnGet()
		{
			var settings = _context.Settings.FirstOrDefault();
			if (settings == null)
			{
				settings = new Settings();
				_context.Settings.Add(settings);
				await _context.SaveChangesAsync();
			}

			Settings.WaitlistEnabled = settings.WaitlistEnabled;

			Waitlist = _context.Waitlist
				.Include(w => w.Profile)
				.ToList()
				.Select(w => new WaitlistDisplayModel
				{
					ProfileId = w.ProfileId,
					Name = $"{w.Profile.FirstName} {w.Profile.LastName}",
					DateJoined = w.DateJoined,
					AllowedPurchase = w.AllowedPurchase,
					Expired = w.AllowedPurchaseDate.AddDays(7) < DateTime.UtcNow
				})
				.ToList();
		}

		public async Task<IActionResult> OnPostSaveSettingsAsync()
		{
			var settings = _context.Settings.FirstOrDefault();
			settings.WaitlistEnabled = Settings.WaitlistEnabled;
			await _context.SaveChangesAsync();

			return RedirectToPage("./Waitlist");
		}

		public async Task<IActionResult> OnPostAllowPurchaseAsync(WaitlistDisplayModel allowPurchase)
		{
			var waitlist = await _context.Waitlist.FirstAsync(w => w.ProfileId == allowPurchase.ProfileId);
			if(!allowPurchase.AllowedPurchase)
			{
				waitlist.AllowedPurchase = false;
				waitlist.AllowedPurchaseDate = default(DateTime);
				await _context.SaveChangesAsync();
			}

			if(!waitlist.AllowedPurchase && allowPurchase.AllowedPurchase)
			{
				waitlist.AllowedPurchase = true;
				waitlist.AllowedPurchaseDate = DateTime.UtcNow;
				await _context.SaveChangesAsync();
				await _emailService.SendWaitlistEmail(allowPurchase.ProfileId);
			}

			return RedirectToPage("./Waitlist");
		}
	}
}