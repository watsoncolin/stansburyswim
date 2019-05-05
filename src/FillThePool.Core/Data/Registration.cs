using Microsoft.AspNetCore.Identity;
using System;

namespace FillThePool.Core.Data
{
	public class Registration
	{
		public int Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public IdentityUser CreatedBy { get; set; }
		virtual public Student Student { get; set; }
		virtual public Transaction Transaction { get; set; }
	}
}
