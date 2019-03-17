using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FillThePool.Models
{
	public class EmailTemplate : IAuditInfo
	{
		public int EmailTemplateId { get; set; }
		[Required]
		public string Type { get; set; }
		public string Subject { get; set; }
		[AllowHtml]
        public string Html { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
	}
}