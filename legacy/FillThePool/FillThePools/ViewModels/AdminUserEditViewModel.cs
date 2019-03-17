using FillThePool.Models;

namespace FillThePool.Web.ViewModels
{
	public class AdminUserEditViewModel
	{
		public User User { get; set; }
		public RoleTypes Role { get; set; }
	
	}

	public enum RoleTypes
	{
		User = 1,
		Instructor = 2,
		Admin = 3
	}
}