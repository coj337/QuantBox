using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeManager.Models;
using Microsoft.AspNetCore.Mvc;
using Trading.API.Data;
using Trading.API.Domain;

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
            return Ok(_context.ExchangeCredentials.Select(x => new { x.Name, x.Nickname, x.PublicKey, x.Simulated }));
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<IEnumerable<ExchangeConfig>> Accounts()
        {
            return Ok(_context.ExchangeCredentials.Select(x => new { x.Name, x.Nickname, x.Simulated }));
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<Dictionary<string, string>> BotAccounts(string botId)
        {
            var bot = _context.Bots.First(x => x.Name == botId);
            var accounts = new Dictionary<string, string>();

            if (bot.Accounts != null)
            {
                foreach (var account in bot.Accounts)
                {
                    accounts.Add(account.Name, account.Nickname);
                }
            }

            return Ok(accounts);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateBotAccount([FromBody]BotAccountUpdate botUpdate)
        {
            var bot = _context.Bots.First(x => x.Name == botUpdate.BotId);
            var account = bot.Accounts.FirstOrDefault(x => x.Nickname == botUpdate.Exchange);
            if (account == null)
            {
                return UnprocessableEntity("Invalid bot name");
            }
            account = _context.ExchangeCredentials.First(x => x.Nickname == botUpdate.Account);

            _context.Bots.Update(bot);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<bool> GetTradingState(string botId)
        {
            return Ok(_context.Bots.First(x => x.Name == botId).TradingEnabled);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SetTradingState([FromBody]BotStateUpdate update)
        {
            var bot = _context.Bots.First(x => x.Name == update.BotId);
            bot.TradingEnabled = update.State;

            _context.Bots.Update(bot);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddExchangeConfig([FromBody]ExchangeConfig config)
        {
            //Check we've been given enough info
            if (string.IsNullOrEmpty(config.Name))
            {
                return UnprocessableEntity("Please choose an exchange");
            }
            if (string.IsNullOrEmpty(config.Nickname))
            {
                return UnprocessableEntity("Please enter a nickname for this account");
            }
            if (!config.Simulated) {
                if (string.IsNullOrEmpty(config.PublicKey))
                {
                    return UnprocessableEntity("Public Key must have a value");
                }
                else if (string.IsNullOrEmpty(config.PrivateKey))
                {
                    return UnprocessableEntity("Private Key must have a value");
                }
            }

            //Make sure we don't already have the creds
            if (_context.ExchangeCredentials.Any(x => x.Name == config.Name && (x.PublicKey == config.PublicKey || x.Nickname == config.Nickname)))
            {
                return UnprocessableEntity("Credentials already exist");
            }

            //Add creds to the db
            _context.ExchangeCredentials.Add(new ExchangeConfig()
            {
                Name = config.Name,
                Nickname = config.Nickname,
                Simulated = config.Simulated,
                PublicKey = config.PublicKey,
                PrivateKey = config.PrivateKey
            });
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RemoveExchangeConfig([FromBody]ExchangeConfig config)
        {
            if (string.IsNullOrEmpty(config.PublicKey) || string.IsNullOrEmpty(config.Name))
            {
                return UnprocessableEntity("Exchange and public key must be provided");
            }

            var configToDelete = _context.ExchangeCredentials.FirstOrDefault(x => x.PublicKey == config.PublicKey && x.Name == config.Name);
            if (configToDelete == null)
            {
                return UnprocessableEntity("Specified key doesn't exist");
            }

            _context.ExchangeCredentials.Remove(configToDelete);
            _context.SaveChanges();

            return Ok();
        }
    }
}
