using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FillThePool.Web.ViewModels
{
	public class AdminUserListViewModel
	{
		public List<AdminUserViewModel> Users { get; set; }
		public bool WaitListEnabled { get; set; }

		public AdminUserListViewModel()
		{
			Users = new List<AdminUserViewModel>();
		}
	}
}