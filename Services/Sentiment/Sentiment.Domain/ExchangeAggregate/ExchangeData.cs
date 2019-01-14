using System;
using System.Collections.Generic;
using System.Text;

namespace Sentiment.Domain.ExchangeAggregate
{
    public class ExchangeData
    {
        public string Exchange { get; set; }
        public string Pair { get; set; }
        public MarketData Data { get; set; }

        public ExchangeData(string exchange, string pair, decimal bid, decimal ask)
        {
            Exchange = exchange;
            Pair = pair;
            Data = new MarketData(bid, ask);
        }
    }
}
