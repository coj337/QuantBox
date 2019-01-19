using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain
{
    public class MarketData
    {
        public string BaseCurrency { get; set; }

        public string AltCurrency { get; set; }

        public decimal Bid { get; set; }

        public decimal Ask { get; set; }

        public decimal BaseVolume { get; set; }

        public decimal AltVolume { get; set; }

        public MarketData()
        {

        }

        public MarketData(decimal bid, decimal ask)
        {
            Bid = bid;
            Ask = ask;
        }

    }
}
