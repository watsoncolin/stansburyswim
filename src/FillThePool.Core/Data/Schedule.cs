using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core.Data
{
	public class Schedule
	{
		public int Id { get; set; }
		[Required]
		public DateTime Start { get; set; }
		[Required]
		public DateTime End { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		[Required]
		virtual public Pool Pool { get; set; }
		[Required]
		virtual public Instructor Instructor { get; set; }
		virtual public Registration Registration { get; set; }
	}

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
