using ExchangeManager.Helpers;
using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Data;
using OrderType = ExchangeManager.Models.OrderType;

namespace Trading.API.Domain.Actions
{
    public class CreateOrder : IAction
    {
        public string Exchange { get; set; }
        public OrderType OrderType { get; set; }
        public ExchangeConfig ExchangeConfig { get; set; }
        public Pair Pair { get; set; }
        public string ExistingCurrency { get; set; }
        public decimal Liquidity { get; set; }
        public long DelayMs { get; set; }
        public bool Simulated { get; set; }

        public async Task<TradeResult> Execute(TradingContext context, IExchange exchange) //TODO: Find a better way to create orders etc. Make this a service?
        {
            if (ExchangeConfig == null)
            {
                return new TradeResult() { ErrorReason = ErrorReason.NoAccount, ErrorMessage = "No account chosen for " + exchange };
            }
            if(exchange == null)
            {
                return new TradeResult() { ErrorReason = ErrorReason.NoExchange, ErrorMessage = "Exchange doesn't exist for " + exchange };
            }
            if (!exchange.TradingEnabled)
            {
                return new TradeResult() { ErrorReason = ErrorReason.TradingDisabled, ErrorMessage = "Trading is currently disabled on exchange: " + exchange };
            }
            
            //TODO: Run checks to make sure coins are enabled etc.
            //TODO: Check that we have enough liquidity for trades (and potentially swap from available liquidity to fill them)

            //If we start on the base, we want to buy the alt
            var side = Pair.BaseCurrency == ExistingCurrency ? OrderSide.Buy : OrderSide.Sell;

            ExchangeOrderResult orderResult;
            if (Simulated)
            {
                orderResult = await exchange.SimulateOrder(Pair.MarketSymbol, side, OrderType.Market, 0, Liquidity, DelayMs); //Use delay to simulate lag between placing and filling the order
            }
            else
            {
                orderResult = await exchange.CreateOrder(Pair.MarketSymbol, side, OrderType.Market, 0, Liquidity);
            }

            TradeResult result = new TradeResult()
            {
                Amount = orderResult.Amount,
                AmountFilled = orderResult.AmountFilled,
                AveragePrice = orderResult.AveragePrice,
                Fees = orderResult.Fees,
                FeesCurrency = orderResult.FeesCurrency,
                FillDate = orderResult.FillDate,
                OrderSide = side,
                MarketSymbol = Pair.MarketSymbol,
                Message = orderResult.Message,
                OrderDate = orderResult.OrderDate,
                OrderId = orderResult.OrderId,
                Price = orderResult.Price,
                Result = orderResult.Result.ToOrderResult(),
                TradeId = orderResult.TradeId
            };

            if (orderResult.Result == ExchangeAPIOrderResult.Canceled || orderResult.Result == ExchangeAPIOrderResult.Error || orderResult.Result == ExchangeAPIOrderResult.Unknown)
            {
                result.ErrorReason = ErrorReason.TradeError;
                result.ErrorMessage = "Something went wrong with order " + orderResult.OrderId + ",\r\nResult:" + orderResult.Result + "\r\n" + orderResult.Message;
            }

            return result;
        }
    }
}
