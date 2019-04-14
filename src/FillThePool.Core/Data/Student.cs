using Microsoft.AspNetCore.Identity;
using System;

namespace FillThePool.Core.Data
{
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
