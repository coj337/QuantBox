using System;
using System.Collections.Generic;
using System.Text;

namespace BtcMarketsApiClient.Models
{
    public class GetMarketsResponse
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<ExchangeMarket> Markets { get; set; }
    }
}
