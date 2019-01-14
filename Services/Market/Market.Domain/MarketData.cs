using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Market.Domain
{
    public class MarketData
    {
        public decimal Bid { get; set; }

        public decimal Ask { get; set; }

        public decimal BaseVolume { get; set; }

        public decimal AltVolume { get; set; }

    }
}
