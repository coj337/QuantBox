using ExchangeManager.Clients;
using ExchangeManager.Helpers;
using ExchangeManager.Models;
using ExchangeSharp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                //Temp bot grabbing so we can check settings (to be called from bot later?)
                Bot bot;

                if (result.Type == ArbitrageType.Triangle)
                {
                    bot = _context.Bots
                        .Include(x => x.Accounts)
                        .First(x => x.Name == "Triangle Arbitrage");
                }
                else
                {
                    return; //Do this later
                    bot = _context.Bots
                        .Include(x => x.Accounts)
                        .First(x => x.Name == "Normal Arbitrage");
                }

                if (bot.TradingEnabled)
                {
                    //Grab bot accounts for this trade
                    var account = bot.Accounts.FirstOrDefault(x => x.Name == result.Exchanges.First());
                    if (account == null)
                    {
                        //_logger.LogWarn("No account chosen for " + result.Exchanges.First());
                        return;
                    }

                    var arbitrageTradeResults = new ArbitrageTradeResults();
                    arbitrageTradeResults.Trades = new List<TradeResult>();
                    arbitrageTradeResults.TimeStarted = DateTime.Now;

                    if (result.Type == ArbitrageType.Triangle)
                    {
                        var ex = _exchanges.First(x => x.Name == result.Exchanges.First()); //Triangle arb only has one exchange
                        if (!account.Simulated && !ex.IsAuthenticated)
                        {
                            throw new Exception("Can't create a trade, user is not authenticated.");
                        }

                        //TODO: Check that we have enough liquidity for trades (and potentially swap from available liquidity to fill them)

                        //Step 1: Buy/Sell initial coins
                        //Step 2: Swap coin into misc coin
                        //Step 3: Swap misc coin back to original
                        var previousCurrency = result.InitialCurrency; //Set start currency for choosing buy/sell
                        var currentLiquidity = result.InitialLiquidity; //The amount of the current asset we have
                        var simDelay = new Random().NextDouble() + 0.1;
                        foreach (var pair in result.Pairs)
                        {
                            //If we start on the base, we want to buy the alt
                            var side = pair.BaseCurrency == previousCurrency ? OrderSide.Buy : OrderSide.Sell;

                            ExchangeOrderResult orderResult;
                            if (account.Simulated)
                            {
                                orderResult = await ex.SimulateOrder(pair.MarketSymbol, side, OrderType.Market, 0, currentLiquidity, simDelay); //Use delay to simulate lag between placing and filling the order
                            }
                            else
                            {
                                orderResult = await ex.CreateOrder(pair.MarketSymbol, side, OrderType.Market, 0, currentLiquidity);
                            }

                            arbitrageTradeResults.BotId = bot.Name;
                            arbitrageTradeResults.Trades.Add(new TradeResult()
                            {
                                Amount = orderResult.Amount,
                                AmountFilled = orderResult.AmountFilled,
                                AveragePrice = orderResult.AveragePrice,
                                Fees = orderResult.Fees,
                                FeesCurrency = orderResult.FeesCurrency,
                                FillDate = orderResult.FillDate,
                                OrderSide = side,
                                MarketSymbol = pair.MarketSymbol,
                                Message = orderResult.Message,
                                OrderDate = orderResult.OrderDate,
                                OrderId = orderResult.OrderId,
                                Price = orderResult.Price,
                                Result = orderResult.Result.ToOrderResult(),
                                TradeId = orderResult.TradeId,
                            });

                            if (orderResult.Result == ExchangeAPIOrderResult.Canceled || orderResult.Result == ExchangeAPIOrderResult.Error || orderResult.Result == ExchangeAPIOrderResult.Unknown)
                            {
                                throw new Exception("Something went wrong with order " + orderResult.OrderId + ",\r\nResult:" + orderResult.Result + "\r\n" + orderResult.Message);
                            }

                            if (side == OrderSide.Buy)
                            {
                                previousCurrency = pair.AltCurrency;
                            }
                            else
                            {
                                previousCurrency = pair.BaseCurrency;
                            }
                            currentLiquidity = orderResult.AmountFilled;
                        }

                        arbitrageTradeResults.InitialCurrency = result.InitialCurrency;
                        arbitrageTradeResults.EstimatedProfit = result.Profit;
                        arbitrageTradeResults.ActualProfit = (arbitrageTradeResults.Trades.Last().AmountFilled - result.InitialLiquidity) / result.InitialLiquidity * 100;

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
                        var trade2 = arbitrageTradeResults.Trades[1];
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

                        arbitrageTradeResults.TimeFinished = DateTime.Now;

                        _context.ArbitrageResults.Add(arbitrageTradeResults);
                        _context.SaveChanges();
                    }
                    else
                    {
                        //TODO: Implemented normal arb
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong executing arbitrage. " + e.Message);
                //_logger.LogCritical("Something went wrong executing arbitrage", ex);
            }
        }
    }
}
