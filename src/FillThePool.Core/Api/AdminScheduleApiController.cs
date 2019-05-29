using System;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using FillThePool.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FillThePool.Core.Api
{
    [Route("api/admin/schedule")]
    [ApiController]
	[Authorize("AdminPolicy")]
	public class AdminScheduleApiController : ControllerBase
    {
		private readonly ApplicationDbContext _context;
		private readonly ScheduleService _scheduleService;
		public AdminScheduleApiController(ApplicationDbContext context, ScheduleService scheduleService)
		{
			_context = context;
			_scheduleService = scheduleService;
		}

		[Route("{strDate}")]
		[HttpGet]
		public async Task<IActionResult> GetScheduleForDay(string strDate)
		{
			var date = DateTime.Parse(strDate);

			var schedules = await _context.Schedules
				.Include("Pool")
				.Include("Instructor")
				.Include(r => r.Registration)
				.Include(r => r.Registration.Student)
				.Where((s) => s.Start.Year == date.Year && s.Start.DayOfYear == date.DayOfYear)
				.ToListAsync();

			return Ok(schedules);
		}


		[Route("{scheduleId}")]
		[HttpDelete]
		public async Task<IActionResult> DeleteSchedule(int scheduleId)
		{
			var schedule = _context.Schedules
				.Include(s => s.Registration)
				.Include(s=> s.Registration.Student.Profile)
				.Where(s => s.Id == scheduleId)
				.FirstOrDefault();

			if(schedule != null && schedule.Registration != null)
			{
				await _scheduleService.Cancel(schedule.Registration.Student.Profile.IdentityUserId, schedule.Registration.Id);
			}
			
			_context.Schedules.Remove(schedule);
			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> CreateSchedule(Schedule newSchedule)
		{
			if(ModelState.IsValid)
			{
				var pool = _context.Pools.Find(newSchedule.Pool.Id);
				var instructor = _context.Instructors.Find(newSchedule.Instructor.Id);

				var schedule = new Schedule
				{
					Start = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(newSchedule.Start, "Mountain Standard Time"),
					End = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(newSchedule.End, "Mountain Standard Time"),
					Pool = pool,
					Instructor = instructor,
					CreatedOn = DateTime.UtcNow,
					ModifiedOn = DateTime.UtcNow,
				};

				_context.Schedules.Add(schedule);
				await _context.SaveChangesAsync();

				return Ok();
			}

			return BadRequest(ModelState);
		}

		[HttpGet]
		[Route("pools")]
		public async Task<IActionResult> GetPools()
		{
			var pools = await _context.Pools.Where(p => p.Active).ToListAsync();
			return Ok(pools);
		}

		[HttpGet]
		[Route("instructors")]
		public async Task<IActionResult> GetInstructors()
		{
			var instructors = await _context.Instructors.Where(p => p.Active).ToListAsync();
			return Ok(instructors);
		}
	}
}