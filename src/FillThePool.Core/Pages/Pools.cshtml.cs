using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Pages
{
    public class PoolsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		public PoolsModel(ApplicationDbContext context)
		{
			_context = context;
		}
		public IList<Pool> Pools { get; set; }
		public void OnGet()
        {
			Pools = _context.Pools.Where((p) => p.Active).ToList();
        }
    }
}