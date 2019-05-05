using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
	public class UserDetailsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		public UserDetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public Profile Profile { get; set; }
		public int Balance { get; set; }
		public IList<Claim> Claims { get; set; } = new List<Claim>();
		public List<Transaction> Transactions { get; set; } = new List<Transaction>();
		public List<Registration> Registrations { get; set; } = new List<Registration>();
		public List<Schedule> Schedules { get; set; } = new List<Schedule>();
		[BindProperty]
		public NewTransactionModel NewTransaction { get; set; } = new NewTransactionModel();
		public class NewTransactionModel
		{
			public string Description { get; set; }
			[DisplayName("Credit")]
			public int LessonCredit { get; set; }
			public int Amount { get; set; }
			public string Type { get; set; }
			public int ProfileId { get; set; }
			public List<string> Types { get; set; } = new List<string>
			{
				"Purchased",
				"Reserved Lesson",
				"Canceled Lesson",
			};
		}

		public enum TransactionTypes
		{
			Purchase
		}

		public async Task OnGet(int profileId)
		{

			NewTransaction.ProfileId = profileId;

			var profile = await _context.Profiles
				.Include(p => p.IdentityUser)
				.Include(p => p.Students)
				.FirstOrDefaultAsync(p => p.Id == profileId);

			var user = await _userManager.FindByIdAsync(profile.IdentityUserId);

			Claims = await _userManager.GetClaimsAsync(user);

			var transactions = _context.Transactions
				.Where(t => t.ProfileId == profile.Id)
				.OrderByDescending(t => t.TimeStamp);

			var registrations = _context.Registrations
				.Include(r => r.Student)
				.Where(r => r.Transaction.ProfileId == profile.Id);


			var schedules = _context.Schedules
				.Include(r => r.Instructor)
				.Include(r => r.Registration)
				.Include(r => r.Registration.Student)
				.Where(r => r.Registration.Student.ProfileId == profile.Id);

			Profile = profile;
			Balance = Utilities.GetAvailableCredits(_context, profile.IdentityUserId);
			Transactions = await transactions.ToListAsync();
			Schedules = await schedules.ToListAsync();
		}

		public async Task<IActionResult> OnPostAddTransactionAsync()
		{
			var transaction = new Transaction
			{
				Description = NewTransaction.Description,
				Amount = NewTransaction.Amount,
				LessonCredit = NewTransaction.LessonCredit,
				TimeStamp = DateTime.UtcNow,
				CreatedOn = DateTime.UtcNow,
				ProfileId = NewTransaction.ProfileId,
			};

			_context.Add(transaction);
			await _context.SaveChangesAsync();

			return RedirectToPage("./UserDetails", new { profileId = NewTransaction.ProfileId });
		}


		public async Task<IActionResult> OnPostLoginAsUserAsync(int profileId)
		{
			var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
			var user = await _userManager.FindByIdAsync(profile.IdentityUserId);
			await _signInManager.SignInAsync(user, false);

			return RedirectToPage("/Index", new { area = "" });
		}
		public async Task<IActionResult> OnPostAddAdminClaimAsync(int profileId)
		{
			var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
			var user = await _userManager.FindByIdAsync(profile.IdentityUserId);

			await _userManager.AddClaimAsync(user, new Claim("Admin", "Full"));

			return RedirectToPage("./UserDetails", new { profileId = NewTransaction.ProfileId });
		}
		public async Task<IActionResult> OnPostRemoveAdminClaimAsync(int profileId)
		{
			var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
			var user = await _userManager.FindByIdAsync(profile.IdentityUserId);

			await _userManager.RemoveClaimAsync(user, new Claim("Admin", "Full"));

			return RedirectToPage("./UserDetails", new { profileId = NewTransaction.ProfileId });
		}
	}
}