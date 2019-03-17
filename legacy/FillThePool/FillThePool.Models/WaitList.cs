using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillThePool.Models
{
	public class WaitList
	{
		public int WaitListId { get; set; }
		public string PurchaseCode { get; set; }
		public DateTime DateAdded { get; set; }
		public virtual User User { get; set; }
		public DateTime? DateCodeAdded { get; set; }

	}
}
