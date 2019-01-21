using System;
using System.Collections.Generic;
using System.Text;

namespace BtcMarketsApiClient.Models
{
    public class GetMarketsResponse
    {
        public bool success { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<Market> markets { get; set; }
    }
}
