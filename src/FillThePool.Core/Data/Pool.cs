using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillThePool.Core.Data
{
    public class Pool
    {
        public int Id { get; set; }
		public bool Active { get; set; }
		public string Name { get; set; }
		public string Details { get; set; }
		public string Address { get; set; }
        public string Image { get; set; }
    }
}
