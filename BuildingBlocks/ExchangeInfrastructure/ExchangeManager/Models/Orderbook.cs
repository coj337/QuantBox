using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeManager.Models
{
    public class Orderbook
    {
        public string Pair { get; set; }

        public string BaseCurrency { get; set; }

        public string AltCurrency { get; set; }

        public List<OrderbookOrder> Bids { get; set; }

        public List<OrderbookOrder> Asks { get; set; }

        public decimal BaseVolume { get; set; }

        public decimal AltVolume { get; set; }
    }
}
