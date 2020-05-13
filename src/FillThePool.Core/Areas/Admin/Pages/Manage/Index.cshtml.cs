using FillThePool.Core.Data;
using FillThePool.Core.Data.Migrations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FillThePool.Core.Areas.Admin.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}


		public IQueryable<UserWithManyCredits> UsersWithManyCredits { get; set; }
		public int LessonCreditCount { get; set; }
		public int AvailableLessons { get; set; }
		public int ScheduledCount { get; set; }
		public int ActiveUsers { get; set; }
		public void OnGet()
		{
			if (User.Claims.Any(c => c.Type == "Admin" && c.Value == "Full"))
			{
				ActiveUsers = _context.Schedules
					.Where(s => s.Registration != null)
					.Select(s => s.Registration.Student.Profile)
					.Distinct()
					.Count();
				LessonCreditCount = _context.Transactions.Where(t => t.LessonCredit > 0).Sum((t) => t.LessonCredit);
				ScheduledCount = _context.Transactions.Where(t => t.LessonCredit < 0).Sum((t) => Math.Abs(t.LessonCredit));
				AvailableLessons = _context.Schedules.Where(s => s.Registration == null).Count();
				UsersWithManyCredits = _context.Transactions.GroupBy(transaction => transaction.Profile).Select(t =>
				new UserWithManyCredits
				{
					Balance = t.Sum(x => x.LessonCredit),
					Profile = t.Key,
				}).Where(t => t.Balance > 20);
			}
		}
	}

	public class UserWithManyCredits
	{
		public int Balance { get; set; }
		public Profile Profile { get; set; }
	}
}