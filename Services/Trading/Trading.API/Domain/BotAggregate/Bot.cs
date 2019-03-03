using ExchangeManager.Models;
using System.Collections.Generic;
using Trading.API.Domain.Actions;
using Trading.API.Domain.BotAggregate;
using Trading.API.Domain.Indicators;
using Trading.API.Domain.Insurances;
using Trading.API.Domain.Safeties;
using Trading.API.Domain.TradeSettingsAggregate;

namespace Trading.API.Domain
{
    public class Bot
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TradingSettings TradeSettings { get; set; }
        public List<BotExchange> Exchanges { get; set; }
        public List<ISafety> Safeties { get; set; }
        public List<IInsurance> Insurances { get; set; }
        public List<IIndicator> Indicators { get; set; } //Indicators to met before executing a trade
        public List<IAction> Actions { get; set; } //Actions to take if conditions from indicators/insurances are met
        public BotBenchmark Benchmarks { get; set; }
        public List<TradeResult> Trades { get; set; } //Trades executed by this bot

        public Bot(string name)
        {
            Name = name;
            Exchanges = new List<BotExchange>();
            Safeties = new List<ISafety>();
            Insurances = new List<IInsurance>();
            Indicators = new List<IIndicator>();
            Trades = new List<TradeResult>();
        }
    }
}
