using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Data;
using Trading.API.Domain;
using Trading.API.Domain.ViewModels;
using Trading.API.Services;

namespace Trading.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BotController : Controller
    {
        private readonly TradingContext _context;
        private readonly ILogger<BotController> _logger;
       // private readonly BotService _botService;

        public BotController(TradingContext context, ILogger<BotController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("All")]
        public ActionResult<IEnumerable<BotPreviewViewModel>> GetAllBots()
        {
            return Ok(_context.Bots.Include(x => x.TradeSettings).Select(x => new BotPreviewViewModel() { Name = x.Name, Profit = 0, TradingEnabled = x.TradeSettings.TradingEnabled }));
        }

        [HttpPost]
        [Route("Create")]
        public ActionResult<Bot> CreateBot(string template)
        {
            var foundTemplate = _context.Templates.FirstOrDefault(x => x.Name == template);
            if(foundTemplate == null)
            {
                return UnprocessableEntity("Template doesn't exist.");
            }

            var bot = new Bot("New Bot")
            {
                Exchanges = foundTemplate.Exchanges
            };
            _context.Bots.Add(bot); //TODO: Populate objects from template once it exists in bots

            return Ok(bot);
        }

        [HttpPost]
        [Route("{botId}/Account")]
        public IActionResult UpdateAccount([FromBody]BotAccountUpdate botUpdate, string botId)
        {
            try
            {
                var bot = _context.Bots
                    .Include(x => x.Exchanges)
                    .FirstOrDefault(x => x.Name == botId);

                if (bot == null)
                {
                    return UnprocessableEntity("Invalid bot name.");
                }

                var config = _context.ExchangeCredentials.FirstOrDefault(x => x.Nickname == botUpdate.Account);
                if (config == null)
                {
                    return UnprocessableEntity("Invalid account name.");
                }

                var account = bot.Exchanges.FirstOrDefault(x => x.Name == botUpdate.Exchange);
                if (account == null)
                {
                    bot.Exchanges.Add(new BotExchange(config.Name) { SelectedConfig = config });
                }
                else
                {
                    account.SelectedConfig = config;
                }

                _context.Bots.Update(bot);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogCritical("UpdateBotAccount threw an exception: " + e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{botId}/Accounts")]
        public ActionResult<Dictionary<string, string>> Accounts(string botId)
        {
            try
            {
                var bot = _context.Bots
                    .Include(x => x.Exchanges)
                    .First(x => x.Name == botId);
                var accounts = new Dictionary<string, string>();

                if (bot.Exchanges != null)
                {
                    foreach (var account in bot.Exchanges)
                    {
                        accounts.Add(account.Name, account.SelectedConfig?.Nickname);
                    }
                }

                return Ok(accounts);
            }
            catch (Exception e)
            {
                _logger.LogCritical("BotAccounts threw an exception: " + e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{botId}/Trades")]
        public ActionResult<IOrderedEnumerable<TradeResults>> GetTrades(string botId)
        {
            try
            {
                var bot = _context.Bots.FirstOrDefault(x => x.Name == botId);
                if (bot == null)
                {
                    return UnprocessableEntity("Can't get trades for a bot that doesn't exist.");
                }
                else
                {
                    return Ok(_context.ArbitrageResults.Include(x => x.Trades).Where(x => x.BotId == botId).OrderByDescending(x => x.TimeFinished).Take(10));
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("GetTrades threw an exception: " + e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{botId}/TradingState")]
        public ActionResult<bool> GetTradingState(string botId)
        {
            var bot = _context.Bots.Include(x => x.TradeSettings).FirstOrDefault(x => x.Name == botId);
            if (bot == null)
            {
                return UnprocessableEntity("Can't get trading state for a bot that doesn't exist.");
            }
            else
            {
                return Ok(bot.TradeSettings.TradingEnabled);
            }
        }

        [HttpPost]
        [Route("{botId}/TradingState")]
        public IActionResult SetTradingState([FromBody]bool state, string botId)
        {
            var bot = _context.Bots.Include(x => x.TradeSettings).First(x => x.Name == botId);
            bot.TradeSettings.TradingEnabled = state;

            _context.Bots.Update(bot);
            _context.SaveChanges();

            return Ok();
        }
    }
}
