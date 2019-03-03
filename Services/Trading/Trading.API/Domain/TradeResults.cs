using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain
{
    public class TradeResults
    {
        public string Id { get; set; }
        public string BotId { get; set; }
        public List<TradeResult> Trades { get; set; }
        public string InitialCurrency { get; set; }
        public decimal EstimatedProfit { get; set; }
        public decimal ActualProfit { get; set; }
        public decimal Dust { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeFinished { get; set; }

        public TradeResults()
        {
            Trades = new List<TradeResult>();
            TimeStarted = DateTime.Now;
        }
    }
}
