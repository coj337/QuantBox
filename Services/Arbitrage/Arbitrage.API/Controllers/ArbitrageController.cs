using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arbitrage.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArbitrageController : ControllerBase
    {
        private readonly ArbitrageService _arbitrageService;

        public ArbitrageController(ArbitrageService arbitrageService)
        {
            _arbitrageService = arbitrageService;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateTriArbThreshold(decimal threshold)
        {
            _arbitrageService.UpdateTriArbThreshold(threshold);
            return Ok();
        }
    }
}
