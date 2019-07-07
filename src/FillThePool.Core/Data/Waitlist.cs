using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core.Data
{
	public class Waitlist
	{
		public int Id { get; set; }
		public int ProfileId { get; set; }
		public Profile Profile { get; set; }
		public DateTime DateJoined { get; set; }
		public bool AllowedPurchase { get; set; }
		public DateTime AllowedPurchaseDate { get; set; }
	}
}
