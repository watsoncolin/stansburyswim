using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
	public class ExportModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public ExportModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public ActionResult OnGet([FromQuery(Name = "active")] bool active)
		{
			List<Profile> profiles;
			if (active)
			{
				profiles = _context.Schedules
					.Where(s => s.Registration != null)
					.Select(s => s.Registration.Student.Profile)
					.Include("IdentityUser")
					.OrderBy(p => p.LastName)
					.Distinct()
					.ToList();
			} else
			{
				profiles = _context.Profiles
				.Include("IdentityUser")
				.OrderBy(p => p.LastName)
				.ToList();
			}

			string result = "FirstName,LastName,Email,Phone,Balance \n";
			foreach (var profile in profiles)
			{
				result += string.Format("{0},{1},{2},{3},{4}\n", profile.FirstName, profile.LastName, profile.IdentityUser.Email, profile.Phone,
					Utilities.GetAvailableCredits(_context, profile.IdentityUserId));
			}

			return File(Encoding.UTF8.GetBytes(result), "text/csv", "stansburyswim.csv");
		}
	}
}