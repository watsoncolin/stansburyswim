using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core.Data
{
	public class Transaction
	{
		public int TransactionId { get; set; }
		public DateTime TimeStamp { get; set; }
		public string PayPalPaymentId { get; set; }
		public string PayPalPayerId { get; set; }
		public string Description { get; set; }
		public int LessonCredit { get; set; }
		public decimal Amount { get; set; }
		public string Type { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }

		virtual public int ProfileId { get; set; }
		virtual public Profile Profile { get; set; }
	}
}
