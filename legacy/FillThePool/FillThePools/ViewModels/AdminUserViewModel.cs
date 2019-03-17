using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FillThePool.Models;

namespace FillThePool.Web.ViewModels
{
	public class AdminUserViewModel
	{
		public User User { get; set; }
		public List<Student> Students { get; set; }
		public int Balance { get; set; }
		public List<Transaction> Transactions { get; set; }


		public AdminUserViewModel()
		{
			Students = new List<Student>();
			Transactions = new List<Transaction>();
		}
	}
}