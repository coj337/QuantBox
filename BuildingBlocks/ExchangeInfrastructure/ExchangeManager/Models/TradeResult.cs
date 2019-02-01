using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeManager.Models
{
    public class TradeResult
    {
        public string Id { get; set; }
        public decimal Price { get; set; }
        public decimal Fees { get; set; }
        public OrderSide OrderSide { get; set; }
        public string MarketSymbol { get; set; }
        public DateTime FillDate { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal AveragePrice { get; set; }
        public string TradeId { get; set; }
        public decimal AmountFilled { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public OrderResult Result { get; set; }
        public string OrderId { get; set; }
        public string FeesCurrency { get; set; }
        public string CorrelationId { get; set; } //Id to link trade with other trades made at the same time
    }

    public enum OrderResult
    {
        Unknown = 0,
        Filled = 1,
        FilledPartially = 2,
        Pending = 3,
        Error = 4,
        Canceled = 5,
        PendingCancel = 6
    }
}
