using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain
{
    public class ArbitrageTradeResults
    {
        public string Id { get; set; }
        public List<TradeResult> Trades { get; set; }
        public string InitialCurrency { get; set; }
        public decimal EstimatedProfit { get; set; }
        public decimal ActualProfit { get; set; }
        public decimal Dust { get; set; }
    }
}
