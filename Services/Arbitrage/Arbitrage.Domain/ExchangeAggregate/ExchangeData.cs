using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain.ExchangeAggregate
{
    public class ExchangeData
    {
        public string Exchange { get; set; }
        public string BaseCurrency { get; set; }
        public string AltCurrency { get; set; }
        public string Pair { get; set; }
        public MarketData Data { get; set; }

        public ExchangeData(string exchange, string pair, string baseCurrency, string altCurrency, decimal bid, decimal ask)
        {
            Exchange = exchange;
            Pair = pair;
            BaseCurrency = baseCurrency;
            AltCurrency = altCurrency;
            Data = new MarketData(bid, ask);
        }
    }
}
