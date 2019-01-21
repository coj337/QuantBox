using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain
{
    public class MarketData
    {
        public string Pair { get; set; }

        public string BaseCurrency { get; set; }

        public string AltCurrency { get; set; }

        public List<Order> Bids { get; set; }

        public List<Order> Asks { get; set; }

        public decimal BaseVolume { get; set; }

        public decimal AltVolume { get; set; }

        public MarketData()
        {

        }

        public MarketData(string pair, string baseCurrency, string altCurrency, List<Order> bids, List<Order> asks)
        {
            Pair = pair;
            BaseCurrency = baseCurrency;
            AltCurrency = altCurrency;
            Bids = bids;
            Asks = asks;
        }

    }
}
