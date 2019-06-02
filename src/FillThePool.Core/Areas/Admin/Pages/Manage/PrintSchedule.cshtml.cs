using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
	public class PrintModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public PrintModel(ApplicationDbContext context)
		{
			_context = context;
		}
		[BindProperty]
		public Filters InputModel { get; set; } = new Filters();
		public List<Schedule> Schedules { get; set; } = new List<Schedule>();
		public List<Pool> Pools { get; set; } = new List<Pool>();
		public List<Instructor> Instructors { get; set; } = new List<Instructor>();

		public class InstructorChoice
		{
			public bool Selected { get; set; }
			public string Instructor { get; set; }
		}
		public class PoolChoice
		{
			public bool Selected { get; set; }
			public string Pool { get; set; }
		}

		public class Filters
		{
			public bool Details { get; set; }
			public DateTime Date { get; set; }
			public List<PoolChoice> PoolChoices { get; set; } = new List<PoolChoice>();
			public List<InstructorChoice> InstructorChoices { get; set; } = new List<InstructorChoice>();
		}
		

		public async Task OnGet()
		{
			InputModel.Date = DateTime.Now;
			Schedules = await GetSchedules(InputModel);
			InputModel.PoolChoices = await GetPools();
			InputModel.InstructorChoices = await GetInstructors();
		}

		public async Task OnPostChangeDateAsync()
		{
			Schedules = await GetSchedules(InputModel);
		}

		private async Task<List<Schedule>> GetSchedules(Filters filters)
		{
			var beginRange = DateTime.Parse(filters.Date.ToShortDateString());
			var endRange = beginRange.AddHours(23).AddMilliseconds(59);

			var query = _context.Schedules
				.Include(s => s.Pool)
				.Include(s => s.Instructor)
				.Include(s => s.Registration)
				.Include(s => s.Registration.Student)
				.Where(s => s.Start > beginRange && s.End < endRange);

			if (filters.PoolChoices.Any(p => p.Selected))
			{
				var names = filters.PoolChoices.Where(p => p.Selected).Select(p => p.Pool);
				query = query.Where(s => names.Contains(s.Pool.Name));
			}

			if (filters.InstructorChoices.Any(p => p.Selected))
			{
				var names = filters.InstructorChoices.Where(p => p.Selected).Select(p => p.Instructor);
				query = query.Where(s => names.Contains(s.Instructor.Name));
			}

			return await query
				.OrderBy(s => s.Start)
				.ToListAsync();
		}

		private async Task<List<PoolChoice>> GetPools()
		{
			return await _context.Pools
				.Where(p => p.Active)
				.OrderBy(s => s.Name)
				.Select(p => new PoolChoice
				{
					Pool = p.Name,
					Selected = false,
				})
				.ToListAsync();
		}
		private async Task<List<InstructorChoice>> GetInstructors()
		{
			return await _context.Instructors
				.Where(p => p.Active)
				.OrderBy(s => s.Name)
				.Select(p => new InstructorChoice
				{
					Instructor = p.Name,
					Selected = false,
				})
				.ToListAsync();
		}
	}
}