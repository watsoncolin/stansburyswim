﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FillThePool.Core.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
		}
		public void OnGet()
		{

			_logger.LogInformation($"oh hai there! : {DateTime.UtcNow}");
			try
			{
				throw new Exception("oops. i haz cause error in UR codez.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "ur code iz buggy.");
			}
		}
	}
}