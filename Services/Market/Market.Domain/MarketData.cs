using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Market.Domain
{
    public class MarketData
    {
        public string Pair { get; set; }

        public string BaseCurrency { get; set; }

        public string AltCurrency { get; set; }

        public decimal Bid { get; set; }

        public decimal Ask { get; set; }

        public decimal BaseVolume { get; set; }

        public decimal AltVolume { get; set; }

    }
}
