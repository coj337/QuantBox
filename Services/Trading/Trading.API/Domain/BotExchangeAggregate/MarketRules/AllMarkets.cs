using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.BotExchangeAggregate.MarketRules
{
    public class AllMarkets : IMarketRule
    {
        public string Name => "All";
    }
}