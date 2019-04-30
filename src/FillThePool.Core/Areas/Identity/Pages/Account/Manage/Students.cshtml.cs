using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FillThePool.Core.Areas.Identity.Pages.Account.Manage
{
    public partial class StudentsModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _context;
		private readonly ILogger<StudentsModel> _logger;

        public StudentsModel(
			ApplicationDbContext context,
			UserManager<IdentityUser> userManager,
			ILogger<StudentsModel> logger)
		{
			_userManager = userManager;
			_context = context;
			_logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

		public List<Student> Students { get; set; } = new List<Student>();

		[BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
			public string Name { get;set;}
			public DateTime Birthday { get; set; }
			public string Ability { get; set; }
			public string Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			var profile = await _context.Profiles
				.Include("Students")
				.FirstOrDefaultAsync(p => p.IdentityUserId == user.Id);

			if (profile == null) {
				return Page();
			}

			Students = profile.Students;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			var profile = _context.Profiles.FirstOrDefault(p => p.IdentityUserId == user.Id);

			if(profile == null)
			{
				profile = new Profile
				{
					IdentityUserId = user.Id
				};
				_context.Profiles.Add(profile);
				await _context.SaveChangesAsync();
			}

			var student = new Student
			{
				ProfileId = profile.Id,
				Name = Input.Name,
				Ability = Input.Ability,
				Birthday = Input.Birthday,
				Notes = Input.Notes
			};

			_context.Students.Add(student);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Added a new student.");

			return RedirectToPage("./Students");
		}


		public async Task<IActionResult> OnPostDeleteStudent(int studentId)
		{
			var user = await _userManager.GetUserAsync(User);
			var student = (await _context.Profiles.Include("Students")
				.FirstOrDefaultAsync(p => p.IdentityUserId == user.Id))
				.Students.FirstOrDefault(s => s.Id == studentId);

			_context.Students.Remove(student);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Students");
		}

		public async Task<IActionResult> OnPostEditStudent(int studentId)
		{
			var user = await _userManager.GetUserAsync(User);
			var student = (await _context.Profiles.Include("Students")
				.FirstOrDefaultAsync(p => p.IdentityUserId == user.Id))
				.Students.FirstOrDefault(s => s.Id == studentId);

			student.Name = Input.Name;
			student.Notes = Input.Notes;
			student.Ability = Input.Ability;
			student.Birthday = Input.Birthday;

			await _context.SaveChangesAsync();

			return RedirectToPage("./Students");
		}
	}
}
