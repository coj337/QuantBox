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
        public ActionResult<IEnumerable<ArbitrageResult>> GetTriangleResults(int limit = 52)
        {
            return Ok(_arbitrageService.triangleResults.OrderByDescending(x => x.Profit).Take(limit));
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<IEnumerable<ArbitrageResult>> GetNormalResults(int limit = 52)
        {
            return Ok(_arbitrageService.normalResults.OrderByDescending(x => x.Profit).Take(limit));
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<ArbitrageResult> GetBestTriangleResult()
        {
            return Ok(_arbitrageService.bestTriangleProfit);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<ArbitrageResult> GetWorstTriangleResult()
        {
            return Ok(_arbitrageService.worstTriangleProfit);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<ArbitrageResult> GetBestNormalResult()
        {
            return Ok(_arbitrageService.bestNormalProfit);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<ArbitrageResult> GetWorstNormalResult()
        {
            return Ok(_arbitrageService.worstNormalProfit);
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
