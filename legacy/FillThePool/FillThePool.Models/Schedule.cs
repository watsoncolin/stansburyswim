using System;
using System.ComponentModel.DataAnnotations;

namespace FillThePool.Models
{
    public class Schedule : IAuditInfo
    {
        public int ScheduleId { get; set; }
		[Required]  
		public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
		[Required]
		public string Lane { get; set; }
        virtual public Registration Registration { get; set; }
	}
}