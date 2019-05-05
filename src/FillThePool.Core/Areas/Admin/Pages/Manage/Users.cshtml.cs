using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
	public class UsersModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public UsersModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public List<ProfileModel> Profiles = new List<ProfileModel>();

		public class ProfileModel
		{
			public Profile Profile { get; set; }
			public int Balance { get; set; }
		}


		public void OnGet(string sort = "LastName", bool ascending = true, int skip = 0, int take = 500)
		{
			var profiles = _context.Profiles
				.Include("IdentityUser")
				.Include("Students")
				.Skip(skip)
				.Take(take);

			Expression<Func<Profile, string>> orderBy;

			switch (sort)
			{
				default:
					{
						orderBy = p => p.LastName;
						break;
					}
			}

			if (ascending)
			{
				profiles = profiles.OrderBy(orderBy);
			}
			else
			{
				profiles = profiles.OrderByDescending(orderBy);
			}


			Profiles = profiles.ToList().Select(p => new ProfileModel
			{
				Profile = p,
				Balance = Utilities.GetAvailableCredits(_context, p.IdentityUserId)
			})
			.ToList();
		}
	}
}