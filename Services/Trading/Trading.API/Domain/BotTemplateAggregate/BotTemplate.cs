using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Domain.Actions;
using Trading.API.Domain.Indicators;
using Trading.API.Domain.Insurances;
using Trading.API.Domain.Safeties;
using Trading.API.Domain.TradeSettingsAggregate;

namespace Trading.API.Domain.BotTemplateAggregate
{
    public class BotTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; } //The templates name
        public TradingSettings TradeSettings { get; set; }
        public List<BotExchange> Exchanges { get; set; }
        public List<ISafety> Safeties { get; set; }
        public List<IInsurance> Insurances { get; set; }
        public List<IIndicator> Indicators { get; set; }
        public List<IAction> Actions { get; set; }

        public BotTemplate(string name)
        {
            Name = name;
            Exchanges = new List<BotExchange>();
            Safeties = new List<ISafety>();
            Insurances = new List<IInsurance>();
            Indicators = new List<IIndicator>();
            Actions = new List<IAction>();
        }
    }
}
