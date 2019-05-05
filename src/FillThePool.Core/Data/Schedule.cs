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
}
