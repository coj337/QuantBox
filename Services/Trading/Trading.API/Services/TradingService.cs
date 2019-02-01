using ExchangeManager.Clients;
using ExchangeManager.Helpers;
using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Data;
using Trading.API.Domain;
using OrderType = ExchangeManager.Models.OrderType;

namespace Trading.API.Services
{
    public class TradingService
    {
        private readonly TradingContext _context;
        private readonly List<IExchange> _exchanges = new List<IExchange>()
        {
            new Binance(),
            new KuCoin(),
            new BtcMarkets(),
            new Coinjar()
        };

        public TradingService(TradingContext context)
        {
            _context = context;
        }

        public async Task ExecuteArbitrage(ArbitrageResult result)
        {
            var arbitrageTradeResults = new ArbitrageTradeResults();
            if (result.Type == ArbitrageType.Triangle)
            {
                var ex = _exchanges.First(x => x.Name == result.Exchanges.First()); //Triangle arb only has one exchange
                if (!ex.IsAuthenticated)
                {
                    throw new Exception("Can't create a trade, user is not authenticated.");
                }

                //TODO: Check that we have enough liquidity for trades (and potentially swap from available liquidity to fill them)

                //Step 1: Buy/Sell initial coins
                //Step 2: Swap coin into misc coin
                //Step 3: Swap misc coin back to original
                foreach (var pair in result.Pairs)
                {
                    var exchangePair = ex.Orderbooks.First(x => x.BaseCurrency == pair.BaseCurrency && x.AltCurrency == pair.AltCurrency || x.BaseCurrency == pair.AltCurrency && x.AltCurrency == pair.BaseCurrency);

                    //If we start on the base, we want to buy the alt
                    var side = exchangePair.BaseCurrency == result.InitialCurrency ? OrderSide.Buy : OrderSide.Sell;

                    ExchangeOrderResult orderResult = await ex.CreateOrder(exchangePair.Pair, side, OrderType.Market, 0, result.InitialLiquidity);
                    arbitrageTradeResults.Trades.Add(new TradeResult()
                    {
                        Amount = orderResult.Amount,
                        AmountFilled = orderResult.AmountFilled,
                        AveragePrice = orderResult.AveragePrice,
                        Fees = orderResult.Fees,
                        FeesCurrency = orderResult.FeesCurrency,
                        FillDate = orderResult.FillDate,
                        OrderSide = side,
                        MarketSymbol = exchangePair.Pair,
                        Message = orderResult.Message,
                        OrderDate = orderResult.OrderDate,
                        OrderId = orderResult.OrderId,
                        Price = orderResult.Price,
                        Result = orderResult.Result.ToOrderResult(),
                        TradeId = orderResult.TradeId,
                    });

                    if(orderResult.Result == ExchangeAPIOrderResult.Canceled || orderResult.Result == ExchangeAPIOrderResult.Error || orderResult.Result == ExchangeAPIOrderResult.Unknown)
                    {
                        throw new Exception("Something went wrong with order " + orderResult.OrderId + ",\r\nResult:" + orderResult.Result + "\r\n" + orderResult.Message);
                    }
                }

                arbitrageTradeResults.InitialCurrency = result.InitialCurrency;
                arbitrageTradeResults.EstimatedProfit = result.Profit;
                arbitrageTradeResults.ActualProfit = arbitrageTradeResults.Trades.Last().AmountFilled;

                //Trade 1 gets converted back to the initial currency and added to the dust collected
                var trade1 = arbitrageTradeResults.Trades.First();
                if (trade1.OrderSide == OrderSide.Buy)
                {
                    arbitrageTradeResults.Dust += trade1.AveragePrice * (trade1.Amount - trade1.AmountFilled);
                }
                else
                {
                    arbitrageTradeResults.Dust += (trade1.Amount - trade1.AmountFilled) / trade1.AveragePrice;
                }

                //Trade 2 gets converted back to the second currency from the first trade and then to the initial currency
                var trade2 = arbitrageTradeResults.Trades.ElementAt(2);
                if (trade2.OrderSide == OrderSide.Buy)
                {
                    var baseAmount = trade2.AveragePrice * (trade2.Amount - trade2.AmountFilled);
                    if (trade1.OrderSide == OrderSide.Buy)
                    {
                        arbitrageTradeResults.Dust += trade1.AveragePrice * baseAmount;
                    }
                    else
                    {
                        arbitrageTradeResults.Dust += baseAmount / trade1.AveragePrice;
                    }
                }
                else
                {
                    var altAmount = (trade2.Amount - trade2.AmountFilled) / trade2.AveragePrice;
                    if (trade1.OrderSide == OrderSide.Buy)
                    {
                        arbitrageTradeResults.Dust += trade1.AveragePrice * altAmount;
                    }
                    else
                    {
                        arbitrageTradeResults.Dust += altAmount / trade1.AveragePrice;
                    }
                }

                //Dust for the final trade is already in the right currency
                var trade3 = arbitrageTradeResults.Trades.Last();
                arbitrageTradeResults.Dust += trade3.Amount - trade3.AmountFilled;

                _context.ArbitrageResults.Add(arbitrageTradeResults);
            }
            else
            {
                //TODO: Implemented normal arb
            }
        }
    }
}
