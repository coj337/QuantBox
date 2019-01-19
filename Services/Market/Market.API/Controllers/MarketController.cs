using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.API.Services;
using Market.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly MarketService _marketService;

        public MarketController(MarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetSupportedExchanges()
        {
            return Ok(_marketService.GetSupportedExchanges());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetSupportedAssets()
        {
            return Ok(_marketService.GetSupportedAssets());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Tickers()
        {
            return Ok(_marketService.GetTickers());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Settings()
        {
            return Ok(_marketService.GetSettings());
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveSettings([FromBody]ExchangeConfig config)
        {
            _marketService.SaveSettings(config);
            return Ok();
        }
    }
}
