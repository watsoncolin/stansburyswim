using Microsoft.AspNetCore.Identity;
using System;

namespace FillThePool.Core.Data
{
	public interface IAuditInfo
	{
		int UserId { get; set; }
		IdentityUser User { get; set; }
		DateTime CreatedOn { get; set; }
		DateTime ModifiedOn { get; set; }
	}
}
