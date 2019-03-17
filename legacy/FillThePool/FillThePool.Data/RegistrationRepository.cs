using FillThePool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillThePool.Data
{
	public class RegistrationRepository : GenericRepository<Registration>
	{
		public RegistrationRepository(DbContext context) : base(context) { }
	}
}
