using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core.Data
{
	public class Product : IAuditInfo
	{
		public int Id { get; set; }
		public int Credits { get; set; }
		public decimal Amount { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public int UserId { get; set; }
		public IdentityUser User { get; set; }
	}
}
