using System;
using System.Web.Mvc;

namespace FillThePool.Models
{
	public class Page : IAuditInfo
	{
		public int PageId { get; set; }
		public string Title { get; set; }
		[AllowHtml]
		public string Html { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }

	}
}
