using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace FillThePool.Core.Data
{
	public class Profile
	{
		public int Id { get; set; }
		[PersonalData]
		public string FirstName { get; set; }
		[PersonalData]
		public string LastName { get; set; }
		[PersonalData]
		public string Referral { get; set; }
		[PersonalData]
		public string Address1 { get; set; }
		[PersonalData]
		public string Address2 { get; set; }
		[PersonalData]
		public string City { get; set; }
		[PersonalData]
		public string State { get; set; }
		[PersonalData]
		public string Zip { get; set; }
		[PersonalData]
		public string Phone { get; set; }
		[PersonalData]
		public bool PrivateRegistration { get; set; }
		public string IdentityUserId { get; set; }
		public virtual IdentityUser IdentityUser { get; set; }
		public List<Student> Students { get; set; } = new List<Student>();
	}

	public class Student
	{
		public int Id { get; set; }
		[PersonalData]
		public string Name { get; set; }
		[PersonalData]
		public DateTime Birthday { get; set; }
		[PersonalData]
		public string Ability { get; set; }
		[PersonalData]
		public string Notes { get; set; }
		public Profile Profile { get; set; }
		public int ProfileId { get; set; }
	}
}
