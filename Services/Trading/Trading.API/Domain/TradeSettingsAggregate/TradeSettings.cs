using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.TradeSettingsAggregate
{
    public class TradingSettings
    {
        public bool TradingEnabled { get; set; }
        public TradeAmount TradeAmount { get; set; } //Amount to buy and settings for that amount (e.g. percentage or fixed)
        public bool Hidden { get; set; } //Use hidden orders if available
        public decimal FeeOverride { get; set; } //Set a custom fee to override the exchange/account default
    }
}
