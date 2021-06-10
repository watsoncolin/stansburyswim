using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FillThePool.Core.Api
{
	[Route("api/schedule")]
	[ApiController]
	[Authorize]
	public class ScheduleApiController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _context;
		private readonly EmailService _emailService;

		public ScheduleApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager, EmailService emailService)
		{
			_context = context;
			_userManager = userManager;
			_emailService = emailService;
		}

		[Route("cancel/{registrationId}")]
		[HttpDelete]
		public async Task<IActionResult> Cancel(int registrationId)
		{
			var user = await _userManager.GetUserAsync(User);
			var scheduleId = -1;

			using (var dbContextTransaction = _context.Database.BeginTransaction())
			{
				try
				{
					var profile = _context.Profiles.First(p => p.IdentityUserId == user.Id);
					var schedule = _context.Schedules.First(s => s.Registration.Student.ProfileId == profile.Id && s.Registration.Id == registrationId);
					scheduleId = schedule.Id;
					var registration = _context.Registrations.First(r => r.Id == registrationId);


					// Prevent cancellation if lesson is less than a day away.
					var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Mountain Standard Time");
					if (schedule.Start < now.AddDays(1))
					{
						return BadRequest();
					}

					_context.Registrations.Remove(registration);

					schedule.Registration = null;

					var transaction = new Transaction
					{
						Amount = -1,
						LessonCredit = 1,
						Description = "Canceled Lesson",
						Profile = profile,
						Type = "Canceled Lesson",
						TimeStamp = DateTime.UtcNow
					};

					_context.Transactions.Add(transaction);
					_context.SaveChanges();

					dbContextTransaction.Commit();
				}
				catch (Exception)
				{
					dbContextTransaction.Rollback();
					ModelState.AddModelError("saving", "Error saving registration");
					return BadRequest(ModelState);
				}
				finally
				{
					await _emailService.SendCancelationEmail(user, scheduleId);
				}
			}

			return Ok();
		}

		[Route("register")]
		[HttpPost]
		public async Task<IActionResult> Register(ScheduleRegistration newRegistration)
		{
			var user = await _userManager.GetUserAsync(User);
			var scheduleId = -1;
			using (var dbContextTransaction = _context.Database.BeginTransaction())
			{
				try
				{
					var credits = Utilities.GetAvailableCredits(_context, user.Id);
					if (credits < 1)
					{
						ModelState.AddModelError("credits", "No credits available");
						return BadRequest(ModelState);
					}

					var profile = _context.Profiles.First(p => p.IdentityUserId == user.Id);
					var schedule = _context.Schedules.First(s => s.Registration == null && s.Id == newRegistration.ScheduleId);
					scheduleId = schedule.Id;
					var student = _context.Students.First(s => s.Id == newRegistration.StudentId);

					var transaction = new Transaction
					{
						Amount = -1,
						LessonCredit = -1,
						Description = "Reserved Lesson",
						Profile = profile,
						Type = "Reserved Lesson",
						TimeStamp = DateTime.UtcNow
					};

					var registration = new Registration
					{
						Student = student,
						TimeStamp = DateTime.UtcNow,
						CreatedBy = user,
						CreatedOn = DateTime.UtcNow,
						Transaction = transaction,
					};

					schedule.Registration = registration;

					_context.Transactions.Add(transaction);
					_context.SaveChanges();

					dbContextTransaction.Commit();
				}
				catch (Exception)
				{
					dbContextTransaction.Rollback();
					ModelState.AddModelError("saving", "Error saving registration");
					return BadRequest(ModelState);
				}
				finally
				{
					await _emailService.SendScheduleEmail(user, scheduleId);
				}
			}

			return Ok();
		}

		[Route("")]
		public async Task<IActionResult> GetAvailbleSchedules()
		{
			var lessons = GetAvailableLessons();

			var poolIds = lessons.Select(l => l.PoolId).Distinct();
			var pools = _context.Pools
				.Where(p => poolIds.Contains(p.Id))
				.ToList();

			var schedule = new ScheduleModel
			{
				Lessons = GetAvailableLessons(),
				DaysWithLessons = await GetDaysWithLessons(),
				Students = await GetStudents(),
				UpcommingLessons = await GetUpcommingLessons(),
				Pools = pools
			};

			return Ok(schedule);
		}

		private async Task<List<int>> GetDaysWithLessons()
		{
			var days = new List<int>();

			foreach (var lesson in await GetUpcommingLessons())
			{
				days.Add(lesson.LessonTime.DayOfYear);
			}

			return days;
		}

		private async Task<List<ScheduledLesson>> GetUpcommingLessons()
		{
			var now = DateTime.Now;
			now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "Mountain Standard Time");
			var user = await _userManager.GetUserAsync(User);
			var profile = _context.Profiles.First(p => p.IdentityUserId == user.Id);
			var schedules = _context.Schedules
				.Include("Registration")
				.Include("Instructor")
				.Include("Pool")
				.Where(s => s.Start > now && s.Registration != null && s.Registration.Student.ProfileId == profile.Id)
				.Select(s => new ScheduledLesson
				{
					Id = s.Registration.Id,
					ScheduleId = s.Id,
					Pool = s.Pool,
					LessonTime = s.Start,
					Instructor = s.Instructor,
					Student = new StudentModel
					{
						Id = s.Registration.Student.Id,
						Name = s.Registration.Student.Name
					}
				})
				.OrderBy(s => s.LessonTime)
				.ToList();

			return schedules;
		}

		private List<Lesson> GetAvailableLessons()
		{
			var now = DateTime.Now;
			now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "Mountain Standard Time");

			var schedules = _context.Schedules.Where(s => s.Start > now && s.Registration == null).Select(s => new Lesson
			{
				Id = s.Id,
				PoolId = s.Pool.Id,
				Instructor = s.Instructor,
				Time = s.Start,
			})
			.OrderBy(s => s.Time)
			.ToList();

			return schedules;
		}

		private async Task<List<StudentModel>> GetStudents()
		{
			var user = await _userManager.GetUserAsync(User);
			var profile = _context.Profiles.First(p => p.IdentityUserId == user.Id);
			return _context.Students.Where(s => s.ProfileId == profile.Id).Select(s => new StudentModel
			{
				Id = s.Id,
				Name = s.Name,
			}).ToList();
		}
	}

	public class StudentModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class ScheduleModel
	{
		public List<ScheduledLesson> UpcommingLessons { get; set; } = new List<ScheduledLesson>();
		public List<Lesson> Lessons { get; set; }
		public List<int> DaysWithLessons { get; set; }
		public List<StudentModel> Students { get; set; }
		public List<Pool> Pools { get; set; }
	}

	public class Lesson
	{
		public int Id { get; set; }
		public DateTime Time { get; set; }
		public Instructor Instructor { get; set; }
		public int PoolId { get; set; }
	}

	public class ScheduledLesson
	{
		public int Id { get; set; }
		public int ScheduleId { get; set; }
		public bool CanCancel
		{
			get
			{
				var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Mountain Standard Time");

				return LessonTime > now.AddDays(1);
			}
		}
		public Pool Pool { get; set; }
		public DateTime LessonTime { get; set; }
		public Instructor Instructor { get; set; }
		public StudentModel Student { get; set; }
	}

	public class ScheduleRegistration
	{
		public int StudentId { get; set; }
		public int ScheduleId { get; set; }
	}
}