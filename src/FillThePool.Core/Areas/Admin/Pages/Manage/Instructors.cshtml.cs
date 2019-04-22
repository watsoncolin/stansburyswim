using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
    public class InstructorModel : PageModel
    {
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly ApplicationDbContext _context;
		public InstructorModel(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}

		public List<Instructor> Instructors { get; set; } = new List<Instructor>();
		[BindProperty]
		public NewInstructorModel NewInstructor { get; set; }

		public class NewInstructorModel
		{
			[Required]
			public string Name { get; set; }
			[Required]
			public string Bio { get; set; }
			[Required]
			public IFormFile Image { get; set; }
		}
		public class EditInstructorModel
		{
			[Required]
			public int Id { get; set; }
			[Required]
			public string Name { get; set; }
			[Required]
			public string Bio { get; set; }
			[Required]
			public IFormFile Image { get; set; }
		}

		public void OnGet()
        {
			Instructors = _context.Instructors.Where((i) => i.Active).ToList();
        }

		public async Task<IActionResult> OnPostAddInstructorAsync()
		{
			var instructor = new Instructor
			{
				Name = NewInstructor.Name,
				Bio = NewInstructor.Bio,
			};
			instructor.Active = true;
			_context.Add(instructor);

			await _context.SaveChangesAsync();
			var ext = Path.GetExtension(NewInstructor.Image.FileName);
			instructor.Image = await Utilities.SaveFile(_hostingEnvironment, NewInstructor.Image, $"instructor-{instructor.Id}.{ext}");
			await _context.SaveChangesAsync();

			return RedirectToPage("./Instructors");
		}

		public async Task<IActionResult> OnPostDeleteInstructorAsync(int instructorId)
		{
			var instructor = _context.Instructors.Find(instructorId);
			if (instructor != null)
			{
				instructor.Active = false;
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Instructors");
		}

		public async Task<IActionResult> OnPostEditInstructorAsync(EditInstructorModel editedInstructor)
		{
			var instructor = _context.Instructors.Find(editedInstructor.Id);
			if (instructor != null)
			{
				if (editedInstructor.Image != null)
				{
					var ext = Path.GetExtension(editedInstructor.Image.FileName);
					instructor.Image = await Utilities.SaveFile(_hostingEnvironment, editedInstructor.Image, $"instructor-{instructor.Id}.{ext}");
					await _context.SaveChangesAsync();
				}

				instructor.Name = editedInstructor.Name;
				instructor.Bio = editedInstructor.Bio;
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Instructors");
		}
	}
}