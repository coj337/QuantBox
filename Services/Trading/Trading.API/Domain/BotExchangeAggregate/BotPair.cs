using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.BotExchangeAggregate
{
    public class BotPair
    {
        public string Id { get; set; }
        public string MarketSymbol { get; set; }
        public string BaseCurrency { get; set; }
        public string AltCurrency { get; set; }
    }
}
