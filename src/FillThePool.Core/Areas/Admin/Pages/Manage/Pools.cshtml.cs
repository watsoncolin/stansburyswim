using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FillThePool.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
	public class PoolsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly IHostingEnvironment _hostingEnvironment;
		public PoolsModel(IHostingEnvironment hostingEnvironment, ApplicationDbContext context)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}
		public List<Pool> Pools { get; set; } = new List<Pool>();
		[BindProperty]
		public NewPoolModel NewPool { get; set; }

		public class NewPoolModel
		{
			[Required]
			public string Name { get; set; }
			[Required]
			public string Details { get; set; }
			[Required]
			public string Address { get; set; }
			[Required]
			public IFormFile Image { get; set; }
		}
		public class EditPoolModel
		{
			[Required]
			public int Id { get; set; }
			[Required]
			public string Name { get; set; }
			[Required]
			public string Details { get; set; }
			[Required]
			public string Address { get; set; }
			[Required]
			public IFormFile Image { get; set; }
		}

		public void OnGet()
		{
			Pools = _context.Pools.Where(p => p.Active).ToList();
		}

		public async Task<IActionResult> OnPostAddPoolAsync()
		{
			var pool = new Pool
			{
				Name = NewPool.Name,
				Address = NewPool.Address,
				Details = NewPool.Details,
			};
			pool.Active = true;
			_context.Add(pool);
			await _context.SaveChangesAsync();
			var ext = Path.GetExtension(NewPool.Image.FileName);
			pool.Image = await Utilities.SaveFile(_hostingEnvironment, NewPool.Image, $"pool-{pool.Id}.{ext}");
			await _context.SaveChangesAsync();

			return RedirectToPage("./Pools");
		}

		public async Task<IActionResult> OnPostDeletePoolAsync(int poolId)
		{
			var pool = _context.Pools.Find(poolId);
			if(pool != null)
			{
				pool.Active = false;
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Pools");
		}

		public async Task<IActionResult> OnPostEditPoolAsync(EditPoolModel editedPool)
		{
			var pool = _context.Pools.Find(editedPool.Id);
			if (pool != null)
			{
				if (editedPool.Image != null)
				{
					var ext = Path.GetExtension(editedPool.Image.FileName);
					pool.Image = await Utilities.SaveFile(_hostingEnvironment, editedPool.Image, $"pool-{pool.Id}{ext}");
					await _context.SaveChangesAsync();
				}

				pool.Name = editedPool.Name;
				pool.Address = editedPool.Address;
				pool.Details = editedPool.Details;
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Pools");
		}
	}
}