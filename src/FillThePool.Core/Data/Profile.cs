using Microsoft.AspNetCore.Identity;
using System;

namespace FillThePool.Core.Data
{
	public class Profile
	{
		public int Id { get; set; }
		public string Referral { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Phone { get; set; }
		public bool PrivateRegistration { get; set; }
		public string IdentityUserId { get; set; }
		public virtual IdentityUser IdentityUser { get; set; }
	}
}
