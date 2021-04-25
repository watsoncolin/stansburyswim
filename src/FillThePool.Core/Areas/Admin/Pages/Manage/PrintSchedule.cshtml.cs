using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
			public string Date { get; set; }
			public List<PoolChoice> PoolChoices { get; set; } = new List<PoolChoice>();
			public List<InstructorChoice> InstructorChoices { get; set; } = new List<InstructorChoice>();
		}
		

		public async Task OnGet()
		{
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
			if (filters.Date == null)
			{
				filters.Date = DateTime.Now.Year + "-W" + DateTime.Now.DayOfYear / 7;
			}
			var weekRegex = new Regex("\\d{4}-W(\\d*)");
			var match = weekRegex.Match(filters.Date);
			var week = int.Parse(match.Groups[1].Value);

			var now = DateTime.Now;
			var jan1 = new DateTime(now.Year, 1, 1);

			var beginRange = jan1.AddDays(-(int)jan1.DayOfWeek).AddDays((week + 1) * 7);
			var endRange = beginRange.AddDays(7);

			var query = _context.Schedules
				.Include(s => s.Pool)
				.Include(s => s.Instructor)
				.Include(s => s.Registration)
				.Include(s => s.Registration.Student)
				.Include(s => s.Registration.Student.Profile)
				.Include(s => s.Registration.Student.Profile.IdentityUser)
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