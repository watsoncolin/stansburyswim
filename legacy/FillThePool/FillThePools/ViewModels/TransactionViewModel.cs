
using System;

namespace FillThePool.Web.ViewModels
{
	public class TransactionViewModel : ViewModelBase
	{
		public decimal Amount { get; set; }
		public string PromoCode { get; set; }
		public string Type { get; set; }
		public int LessonCredit { get; set; }
		public string Description { get; set; }
		public int UserId { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}