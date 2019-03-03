using ExchangeManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Data;
using Trading.API.Domain;

namespace Trading.API.Services
{
    public class BotService
    {
        private readonly PriceService _priceService;
        private readonly TradingContext _context;
        private readonly ILogger<BotService> _logger;

        public BotService(TradingContext context, ILogger<BotService> logger, PriceService priceService)
        {
            _priceService = priceService;
            _context = context;
            _logger = logger;
        }

        public void CreateBot(string name)
        {
            _context.Bots.Add(new Bot(name));
            _context.SaveChanges();
        }

        public void DeleteBot(string name)
        {
            _context.Bots.Remove(_context.Bots.First(x => x.Name == name));
            _context.SaveChanges();
        }

        public void UpdateName(string oldName, string newName)
        {
            var bot = _context.Bots.First(x => x.Name == oldName);
            bot.Name = newName;

            _context.Bots.Update(bot);
            _context.SaveChanges();
        }

        public void UpdateTradingState(string name, bool state)
        {
            var bot = _context.Bots.Include(x => x.TradeSettings).First(x => x.Name == name);
            bot.TradeSettings.TradingEnabled = state;

            _context.Bots.Update(bot);
            _context.SaveChanges();
        }

        public async Task RunBot(string botName)
        {
            try
            {
                var bot = _context.Bots
                        .Include(x => x.TradeSettings)
                        .Include(x => x.Safeties)
                        .Include(x => x.Indicators)
                        .Include(x => x.Actions)
                        .Include(x => x.Trades)
                        .First(x => x.Name == botName);

                if (bot.TradeSettings.TradingEnabled)
                {
                    //Safeties should be triggered independantly before everything else
                    foreach(var safety in bot.Safeties)
                    {
                        if (safety.IsValid())
                        {
                            await safety.TriggerAction();
                            return; //TODO: Do any safeties require continuing?
                        }
                    }

                    foreach(var indicator in bot.Indicators)
                    {
                        if (indicator.IsValid(_priceService.GetExchanges()))
                        {
                            foreach(var insurance in bot.Insurances)
                            {
                                if (insurance.IsValid())
                                {
                                    return; //Insurance stopped trade
                                }
                            }
                        }
                        else
                        {
                            return; //Indicator stopped trade
                        }
                    }

                    //Execute bot actions if we pass all the checks
                    foreach (var action in bot.Actions)
                    {
                        try
                        {
                            var actionExchange = _priceService.GetExchanges().First(x => x.Name == action.Exchange);
                            var tradeResult = await action.Execute(_context, actionExchange);
                            bot.Trades.Add(tradeResult);
                        }
                        catch(Exception e)
                        {
                            _logger.LogCritical("Exception thrown trying to execute an action.", e);
                        }
                    }
                    _context.Update(bot);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("Something went wrong running the " + botName + " bot.", e);
            }
        }
    }
}
