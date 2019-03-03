using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.TradeSettingsAggregate
{
    public class TradeAmount
    {
        public TradeAmountType AmountType { get; set; }
        public decimal Amount { get; set; } //How much the bot uses per-trade
        public decimal MaxAmount { get; set; } //The amount amount the bot is allowed to use at once
    }

    public enum TradeAmountType
    {
        Percentage,
        Fixed
    }
}
