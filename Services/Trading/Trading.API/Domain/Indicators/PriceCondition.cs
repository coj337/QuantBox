using ExchangeManager.Helpers;
using ExchangeManager.Models;
using System.Collections.Generic;
using System.Linq;

namespace Trading.API.Domain.Indicators
{
    public class PriceCondition : IIndicator
    {
        public string StartExchange;
        public string StartPair;
        public OrderSide StartSide;

        public string EndExchange;
        public string EndPair;
        public OrderSide EndSide;

        public PriceConditionType ConditionType;
        public decimal Amount; //Amount to currency to be bought (needed to work down the orderbook)

        public bool IsValid(List<IExchange> exchanges)
        {
            decimal startPrice, endPrice;
            var startExchange = exchanges.First(x => x.Name == StartExchange);
            var endExchange = exchanges.First(x => x.Name == EndExchange);

            //Calculate start price
            if (StartSide == OrderSide.Buy)
            {
                var asks = startExchange.Orderbooks[StartPair].Asks;
                if (Amount == 0)
                {
                    startPrice = asks.First().Price;
                }
                else
                {
                    startPrice = PriceCalculator.GetPriceQuote(asks, PriceCalculator.ConvertBaseToAlt(asks.First().Price, Amount));
                }
            }
            else
            {
                var bids = startExchange.Orderbooks[StartPair].Bids;
                if (Amount == 0)
                {
                    startPrice = bids.First().Price;
                }
                else
                {
                    startPrice = PriceCalculator.GetPriceQuote(bids, Amount);
                }
            }

            //Calculate end price
            if (EndSide == OrderSide.Buy)
            {
                var asks = endExchange.Orderbooks[EndPair].Asks;
                if (Amount == 0)
                {
                    endPrice = asks.First().Price;
                }
                else
                {
                    endPrice = PriceCalculator.GetPriceQuote(asks, PriceCalculator.ConvertBaseToAlt(asks.First().Price, Amount));
                }
            }
            else
            {
                var bids = endExchange.Orderbooks[EndPair].Bids;
                if (Amount == 0)
                {
                    endPrice = bids.First().Price;
                }
                else
                {
                    endPrice = PriceCalculator.GetPriceQuote(bids, Amount);
                }
            }

            if (ConditionType == PriceConditionType.Higher)
            {
                return startPrice > endPrice;
            }
            else
            {
                return startPrice < endPrice;
            }
        }
    }

    public enum PriceConditionType
    {
        Higher,
        Lower
    }
}
