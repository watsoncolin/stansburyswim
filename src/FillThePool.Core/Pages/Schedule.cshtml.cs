using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FillThePool.Core.Pages
{
    public class ScheduleModel : PageModel
    {
		PayPalOptions _paypalOptions;
		public ScheduleModel(IOptions<PayPalOptions> paypalOptions)
		{
			_paypalOptions = paypalOptions.Value;
		}

		public string PayPalClientId => _paypalOptions.PayPalClientId;

        public void OnGet()
        {

        }
    }
}