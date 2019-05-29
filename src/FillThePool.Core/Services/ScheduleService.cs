using FillThePool.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core.Services
{
	public class ScheduleService
	{
		private readonly ApplicationDbContext _context;
		private readonly EmailService _emailService;
		public ScheduleService(ApplicationDbContext context, EmailService emailService)
		{
			_context = context;
			_emailService = emailService;
		}
		public async Task<bool> Cancel(string identityUserId, int registrationId)
		{
			var scheduleId = -1;
			using (var dbContextTransaction = _context.Database.BeginTransaction())
			{
				var user = _context.Users.FirstOrDefault(u => u.Id == identityUserId);
				try
				{
					var profile = _context.Profiles.First(p => p.IdentityUserId == identityUserId);
					var schedule = _context.Schedules.First(s => s.Registration.Student.ProfileId == profile.Id && s.Registration.Id == registrationId);
					scheduleId = schedule.Id;
					var registration = _context.Registrations.First(r => r.Id == registrationId);

					// Prevent cancellation if lesson is less than a day away.
					var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Mountain Standard Time");

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
					return false;
				}
				finally
				{
					await _emailService.SendCancelationEmail(user, scheduleId);
				}
			}


			return true;
		}
	}
}
