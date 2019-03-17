using System.Net;
using System.Net.Mail;
using SendGridMail;

namespace FillThePool.Web
{
	public static class Email
	{
		public static void SendEmail(string toAddress, string subject, string message)
		{
			var fromEmail = Config.SendFrom;
			var siteName = Config.SiteName;
			// Create an email, passing in the the eight properties as arguments.
			var myMessage = SendGrid.GetInstance();
			// Setup the email properties.
			myMessage.From = new MailAddress(fromEmail, siteName);
			myMessage.Subject = subject;
			myMessage.AddTo(toAddress);
			myMessage.Html = message;
			myMessage.AddBcc("info@arnellaquatics.com");

			// Create network credentials to access your SendGrid account.
			var username = Config.SendGridUser;
			var pswd = Config.SendGridPassword;

			var credentials = new NetworkCredential(username, pswd);

			// Create a Web transport for sending email.
			var transportWeb = SendGridMail.Web.GetInstance(credentials);

			// Send the email.
			if (transportWeb != null)
				transportWeb.Deliver(myMessage);
		}
	}
}