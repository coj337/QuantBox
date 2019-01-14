using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly MarketService _marketService;

        public MarketController(MarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet]
        public IActionResult GetSupportedExchanges()
        {
            return Ok(_marketService.GetSupportedExchanges());
        }

        [HttpGet]
        public IActionResult GetSupportedAssets()
        {
            return Ok(_marketService.GetSupportedAssets());
        }
    }
}
