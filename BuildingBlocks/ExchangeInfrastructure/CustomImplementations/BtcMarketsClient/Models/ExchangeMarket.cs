using System;
using System.Collections.Generic;
using System.Text;

namespace BtcMarketsApiClient.Models
{
    public class ExchangeMarket
    {
        public string Pair { get; set; }
        public string Instrument { get; set; }
        public string Currency { get; set; }
    }
}
