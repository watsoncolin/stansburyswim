using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FillThePool.Data;
using FillThePool.Models;
using WebMatrix.WebData;

namespace FillThePool.Web.ViewModels
{
	public class WaitListViewModel
	{
		[Display(Name = "Join Wait list")]
		public bool OnWaitList
		{
			get
			{
				var unit = new ApplicationUnit();
				return unit.WaitList.GetAll().Any(w => w.User.UserName == WebSecurity.CurrentUserName);
			}
		}

		public string Code
		{
			get
			{
				var unit = new ApplicationUnit();

				foreach (var waitList in unit.WaitList.GetAll())
				{
					if (waitList.DateCodeAdded < DateTime.Now.AddDays(-7))
						unit.WaitList.Delete(waitList);
				}
				unit.SaveChanges();

				var firstOrDefault = unit.WaitList.GetAll().FirstOrDefault(w => w.User.UserName == WebSecurity.CurrentUserName);
				return firstOrDefault != null ? firstOrDefault.PurchaseCode : "";
			}
		}
	}
}