using FillThePool.Core.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;

namespace FillThePool.Core.Areas.Admin.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}


		public int LessonCreditCount { get; set; }
		public int ScheduledCount{ get; set; }
		public void OnGet()
		{
			if(User.Claims.Any(c => c.Type == "Admin" && c.Value == "Full"))
			{
				LessonCreditCount = _context.Transactions.Where(t => t.LessonCredit > 0).Sum((t) => t.LessonCredit);
				ScheduledCount = _context.Transactions.Where(t => t.LessonCredit < 0).Sum((t) => Math.Abs(t.LessonCredit));
			}
		}
	}
}