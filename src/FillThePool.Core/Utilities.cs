using FillThePool.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core
{
	public static class Utilities
	{
		public static async Task<string> SaveFile(IHostingEnvironment hostingEnvironment, IFormFile file, string fileName)
		{
			var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");

			var uploadDirectory = new DirectoryInfo(uploads);
			if(!uploadDirectory.Exists)
			{
				uploadDirectory.Create();
			}

			var filePath = Path.Combine(uploads, fileName);
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}
			
			return $"/uploads/{fileName}";
		}

		public static int GetAvailableCredits(ApplicationDbContext context, string userId)
		{
			var profile = context.Profiles.FirstOrDefault(p => p.IdentityUserId == userId);

			if (profile == null)
				return 0;

			var credits = context.Transactions.Where(t => t.ProfileId == profile.Id).Sum(t => t.LessonCredit);

			return credits;
		}
	}
}
