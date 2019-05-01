using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core
{
	public class EmailSenderOptions
	{
		public string SendGridUser { get; set; }
		public string SendGridKey { get; set; }
	}

	public class EmailSender : IEmailSender
	{
		public EmailSender(IOptions<EmailSenderOptions> optionsAccessor)
		{
			Options = optionsAccessor.Value;
		}

		public EmailSenderOptions Options { get; } //set only via Secret Manager

		public Task SendEmailAsync(string email, string subject, string message)
		{
			return Execute(Options.SendGridKey, subject, message, email);
		}

		public Task Execute(string apiKey, string subject, string message, string email)
		{
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage
			{
				From = new EmailAddress("info@stansburyswim.com", "Stansbury Swim"),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message,
			};
			msg.AddTo(new EmailAddress(email));

			msg.SetClickTracking(true, true);

			return client.SendEmailAsync(msg);
		}
	}
}
