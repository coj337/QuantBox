using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arbitrage.Domain;
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
        public ActionResult<IEnumerable<ArbitrageResult>> GetCurrentResults(int limit = 52)
        {
            return Ok(_arbitrageService.currentResults.OrderByDescending(x => x.Profit).Take(limit));
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<IEnumerable<ArbitrageResult>> GetTriangleResults(int limit = 52)
        {
            return Ok(_arbitrageService.profitableTransactions.OrderByDescending(x => x.Profit).Take(limit));
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<ArbitrageResult> GetBestResult()
        {
            return Ok(_arbitrageService.bestProfit);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<ArbitrageResult> GetWorstResult()
        {
            return Ok(_arbitrageService.worstProfit);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<decimal> GetTriangleThreshold()
        {
            return Ok(_arbitrageService.GetTriangleThreshold());
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult UpdateTriangleThreshold(decimal threshold)
        {
            _arbitrageService.UpdateTriArbThreshold(threshold);
            return Ok();
        }
    }
}
