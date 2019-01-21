using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain.ExchangeAggregate
{
    public class ExchangeData
    {
        public string Exchange { get; set; }
        public MarketData Data { get; set; }

        public ExchangeData(string exchange, string pair, string baseCurrency, string altCurrency, List<Order> bids, List<Order> asks)
        {
            Exchange = exchange;
            
            Data = new MarketData(pair, baseCurrency, altCurrency, bids, asks);
        }
    }
}
