using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FillThePool.Core.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
		private readonly ApplicationDbContext _context;

        public DownloadPersonalDataModel(
            UserManager<IdentityUser> userManager,
			ApplicationDbContext context,
            ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
			_context = context;
			_logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, object>();
            var personalDataProps = typeof(IdentityUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

			// Add profile data
			var profile = _context.Profiles.Include("Students").FirstOrDefault(p => p.IdentityUserId == user.Id);
			if (profile != null)
			{
				var profileObj = new Dictionary<string, object>();
				var profileDataProps = typeof(Profile).GetProperties().Where(
							   prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
				foreach (var p in profileDataProps)
				{
					profileObj.Add(p.Name, p.GetValue(profile)?.ToString() ?? null);
				}
				personalData.Add("profile", profileObj);
			}
			// Add students
			if (profile != null && profile.Students != null)
			{
				var studentArrayObjs = new List<Dictionary<string, string>>();
				var studentDataProps = typeof(Student).GetProperties().Where(
							   prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
				foreach (var student in profile.Students)
				{
					var studentObj = new Dictionary<string, string>();
					foreach (var p in studentDataProps)
					{
						studentObj.Add(p.Name, p.GetValue(student)?.ToString() ?? null);
					}
					studentArrayObjs.Add(studentObj);
				}
				personalData.Add("students", studentArrayObjs);
			}

			Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }
    }
}
