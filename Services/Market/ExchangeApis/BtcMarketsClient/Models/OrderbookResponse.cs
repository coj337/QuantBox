using System;
using System.Collections.Generic;
using System.Text;

namespace BtcMarketsApiClient.Models
{
    public class OrderbookResponse
    {
        public string currency { get; set; }
        public string instrument { get; set; }
        public long timestamp { get; set; }
        public List<List<decimal>> asks { get; set; }
        public List<List<decimal>> bids { get; set; }
    }
}
