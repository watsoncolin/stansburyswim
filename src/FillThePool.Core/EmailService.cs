using FillThePool.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core
{
	public class EmailService
	{
		private readonly EmailSenderOptions _senderOptions;
		private readonly ApplicationDbContext _context;
		public EmailService(IOptions<EmailSenderOptions> senderOptions, ApplicationDbContext context)
		{
			_context = context;
			_senderOptions = senderOptions.Value;
		}

		public async Task SendRegistrationEmail(IdentityUser identityUser)
		{
			var client = new SendGridClient(_senderOptions.SendGridKey);

			var emailTemplate = _context.EmailTemplates.First(e => e.Type == "Registration Email");
			var user = _context.Users.First(u => u.Id == identityUser.Id);

			var msg = new SendGridMessage
			{
				From = new EmailAddress("info@stansburyswim.com", "Stansbury Swim"),
				Subject = emailTemplate.Subject,
				HtmlContent = emailTemplate.Html,
			};
			msg.AddTo(new EmailAddress(user.Email));
			msg.AddBccs(_senderOptions.BccEmails.Select(e => new EmailAddress(e)).ToList());

			msg.SetClickTracking(true, true);

			await client.SendEmailAsync(msg);
		}

		public async Task SendPurchaseEmail(IdentityUser identityUser)
		{
			var client = new SendGridClient(_senderOptions.SendGridKey);

			var emailTemplate = _context.EmailTemplates.First(e => e.Type == "Purchase");
			var user = _context.Users.First(u => u.Id == identityUser.Id);

			var msg = new SendGridMessage
			{
				From = new EmailAddress("info@stansburyswim.com", "Stansbury Swim"),
				Subject = emailTemplate.Subject,
				HtmlContent = emailTemplate.Html,
			};
			msg.AddTo(new EmailAddress(user.Email));
			msg.AddBccs(_senderOptions.BccEmails.Select(e => new EmailAddress(e)).ToList());

			msg.SetClickTracking(true, true);

			await client.SendEmailAsync(msg);
		}

		public async Task SendScheduleEmail(IdentityUser identityUser, int scheduleId)
		{
			var client = new SendGridClient(_senderOptions.SendGridKey);

			var emailTemplate = _context.EmailTemplates.First(e => e.Type == "Schedule");
			var user = _context.Users.First(u => u.Id == identityUser.Id);

			var schedule = await _context.Schedules
				.Include("Pool")
				.Include("Registration")
				.Include("Registration.Student")
				.Include("Instructor")
				.Where(s => s.Id == scheduleId)
				.FirstAsync();

			var body = emailTemplate.Html.Replace("{{student}}", schedule.Registration.Student.Name);
			body = body.Replace("{{date}}", schedule.Start.ToShortDateString());
			body = body.Replace("{{time}}", schedule.Start.ToShortTimeString());
			body = body.Replace("{{location}}", schedule.Pool.Address);
			body = body.Replace("{{instructor}}", schedule.Instructor.Name);

			var msg = new SendGridMessage
			{
				From = new EmailAddress("info@stansburyswim.com", "Stansbury Swim"),
				Subject = emailTemplate.Subject,
				HtmlContent = body,
			};
			msg.AddTo(new EmailAddress(user.Email));

			msg.SetClickTracking(true, true);

			await client.SendEmailAsync(msg);
		}

		public async Task SendCancelationEmail(IdentityUser identityUser, int scheduleId)
		{
			var client = new SendGridClient(_senderOptions.SendGridKey);

			var emailTemplate = _context.EmailTemplates.First(e => e.Type == "Cancellation");
			var user = _context.Users.First(u => u.Id == identityUser.Id);

			var schedule = await _context.Schedules
				.Include("Pool")
				.Include("Instructor")
				.Where(s => s.Id == scheduleId)
				.FirstAsync();

			var body = emailTemplate.Html.Replace("{{date}}", schedule.Start.ToShortDateString());
			body = body.Replace("{{time}}", schedule.Start.ToShortTimeString());
			body = body.Replace("{{location}}", schedule.Pool.Address);
			body = body.Replace("{{instructor}}", schedule.Instructor.Name);

			var msg = new SendGridMessage
			{
				From = new EmailAddress("info@stansburyswim.com", "Stansbury Swim"),
				Subject = emailTemplate.Subject,
				HtmlContent = body,
			};
			msg.AddTo(new EmailAddress(user.Email));
			msg.AddBccs(_senderOptions.BccEmails.Select(e => new EmailAddress(e)).ToList());

			msg.SetClickTracking(true, true);

			await client.SendEmailAsync(msg);
		}
		public async Task SendWaitlistEmail(int profileId)
		{
			var client = new SendGridClient(_senderOptions.SendGridKey);

			var emailTemplate = _context.EmailTemplates.First(e => e.Type == "Waitlist");
			var profile = await _context.Profiles.FirstAsync(p => p.Id == profileId);
			var user = await _context.Users.FirstAsync(u => u.Id == profile.IdentityUserId);

			var body = emailTemplate.Html;
			var msg = new SendGridMessage
			{
				From = new EmailAddress("info@stansburyswim.com", "Stansbury Swim"),
				Subject = emailTemplate.Subject,
				HtmlContent = body,
			};
			msg.AddTo(new EmailAddress(user.Email));
			msg.AddBccs(_senderOptions.BccEmails.Select(e => new EmailAddress(e)).ToList());

			msg.SetClickTracking(true, true);

			await client.SendEmailAsync(msg);
		}
	}
}
