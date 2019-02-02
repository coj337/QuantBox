using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeManager.Models;
using Microsoft.AspNetCore.Mvc;
using Trading.API.Data;

namespace Trading.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        private readonly TradingContext _context;

        public SettingsController(TradingContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<IEnumerable<string>> SupportedExchanges()
        {
            return Ok(_context.Exchanges.Select(x => x.Name));
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<IEnumerable<ExchangeConfig>> ExchangeConfigs()
        {
            return Ok(_context.ExchangeCredentials);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddExchangeConfig(string exchange, string publicKey, string privateKey)
        {
            _context.ExchangeCredentials.Add(new ExchangeConfig()
            {
                Name = exchange,
                PublicKey = publicKey,
                PrivateKey = privateKey
            });
            _context.SaveChanges();

            return Ok();
        }
    }
}
