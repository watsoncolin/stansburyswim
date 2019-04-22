using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Pages
{
    public class InstructorsModel : PageModel
    {
		private readonly ApplicationDbContext _context;
		public InstructorsModel(ApplicationDbContext context)
		{
			_context = context;
		}


		public List<Instructor> Instructors { get; set; } = new List<Instructor>();

		public void OnGet()
        {
			Instructors = _context.Instructors.Where((i) => i.Active).ToList();
        }
    }
}