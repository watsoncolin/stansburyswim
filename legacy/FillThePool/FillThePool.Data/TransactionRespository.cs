using FillThePool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillThePool.Data
{
	public class TransactionRespository : GenericRepository<Transaction>
	{
		public TransactionRespository(DbContext context) : base(context) { }
	}
}
