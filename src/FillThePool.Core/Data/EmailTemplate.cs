using System;
using System.ComponentModel.DataAnnotations;

namespace FillThePool.Core.Data
{
	public class EmailTemplate
	{
		public int Id { get; set; }
		[Required]
		public string Type { get; set; }
		public string Subject { get; set; }
        public string Html { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
	}
}