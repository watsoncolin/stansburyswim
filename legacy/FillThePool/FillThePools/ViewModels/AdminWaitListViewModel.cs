using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FillThePool.Data;
using FillThePool.Models;

namespace FillThePool.Web.ViewModels
{
	public class AdminWaitListViewModel
	{
		public WaitList WaitList { get; set; }

		public bool CurrentStudent
		{
			get
			{
				var unit = new ApplicationUnit();
				return unit.Registrations.GetAll().Any(r => r.Student.User.UserName == WaitList.User.UserName);
			}
		}

	}
}