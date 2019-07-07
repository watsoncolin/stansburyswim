using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core.Data
{
	public static class DbInitializer
	{
		public static void Initialize(ApplicationDbContext context)
		{
			context.Database.Migrate();

			var waitlistTemplate = context.EmailTemplates.FirstOrDefault(t => t.Type == "Waitlist");
			if(waitlistTemplate == null)
			{
				waitlistTemplate = new EmailTemplate
				{
					Type = "Waitlist",
					Subject = "Stansbury Swim Lessons Available",
					Html = "You can now purchase more credits at Stansbury Swim. This invitation is valid for 7 days.  You'll be able to make one transaction then you'll need to add yourself back to the wait list if you need more.  <a href='https://www.stansburyswim.com/pricing'>Click here to purchase more credits.</a>",
					CreatedOn = DateTime.UtcNow,
					ModifiedOn = DateTime.UtcNow
				};
				context.EmailTemplates.Add(waitlistTemplate);
				context.SaveChanges();
			}

		}
	}
}
