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

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTriangleResults()
        {
            return Ok(_arbitrageService.profitableTransactions);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetBestResult()
        {
            return Ok(_arbitrageService.bestProfit);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetWorstResult()
        {
            return Ok(_arbitrageService.worstProfit);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTriangleThreshold()
        {
            return Ok(_arbitrageService.GetTriangleThreshold());
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateTriangleThreshold(decimal threshold)
        {
            _arbitrageService.UpdateTriArbThreshold(threshold);
            return Ok();
        }
    }
}
