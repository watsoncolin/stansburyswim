using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FillThePool.Core.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleApiController : ControllerBase
    {
		public async Task<IActionResult> GetAvailbleSchedules()
		{

			return Ok();
		}
    }
}