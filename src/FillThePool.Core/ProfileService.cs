using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core
{
	public class ProfileService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		public ProfileService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<bool> IsProfileComplete(IdentityUser user)
		{
			var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

			if (profile == null)
				return false;
			if (string.IsNullOrEmpty(profile.FirstName))
				return false;
			if (string.IsNullOrEmpty(profile.LastName))
				return false;
			if (string.IsNullOrEmpty(profile.Phone))
				return false;
			if (string.IsNullOrEmpty(profile.Address1))
				return false;
			if (string.IsNullOrEmpty(profile.City))
				return false;
			if (string.IsNullOrEmpty(profile.Zip))
				return false;
			if (string.IsNullOrEmpty(profile.State))
				return false;
			if (!await _userManager.IsEmailConfirmedAsync(user))
				return false;

			return true;
		}
	}
}
