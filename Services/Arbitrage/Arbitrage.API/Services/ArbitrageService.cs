using Arbitrage.API.IntegrationEvents.Events;
using Arbitrage.Domain;
using BuildingBlocks.EventBus.Abstractions;
using ExchangeManager.Clients;
using ExchangeManager.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrage.Api.Services
{
    public class ArbitrageService : BackgroundService
    {
        private readonly IEventBus _eventBus;
        //private readonly IHubContext<ArbitrageHub, IArbitrageHub> _arbitrageHub;

        public readonly List<ArbitrageResult> triangleResults = new List<ArbitrageResult>();
        public readonly List<ArbitrageResult> normalResults = new List<ArbitrageResult>();

        public readonly List<ArbitrageResult> profitableTriangleResults = new List<ArbitrageResult>();
        public readonly List<ArbitrageResult> profitableNormalResults = new List<ArbitrageResult>();

        public ArbitrageResult bestTriangleProfit = new ArbitrageResult() { Profit = -101 };
        public ArbitrageResult worstTriangleProfit = new ArbitrageResult() { Profit = 101 };
        public ArbitrageResult bestNormalProfit = new ArbitrageResult() { Profit = -101 };
        public ArbitrageResult worstNormalProfit = new ArbitrageResult() { Profit = 101 };

        private decimal _profitThreshold = 1.0025m; //0.25% profit default
        private bool TradingEnabled { get; set; }
        private readonly List<IExchange> _exchanges = new List<IExchange>()
        {
            new Binance(),
            //new KuCoin(),
            new BtcMarkets(),
            new Coinjar()
        };


        public ArbitrageService(IEventBus eventBus/*, IHubContext<ArbitrageHub, IArbitrageHub> arbitrageHub*/)
        {
            _eventBus = eventBus;
            //_arbitrageHub = arbitrageHub;

            this.TradingEnabled = false;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var exchange in _exchanges)
            {
                exchange.StartOrderbookListener();
            }

            Task.Run(() =>
                StartTriangleArbitrageListener()
            );

            Task.Run(() =>
                StartNormalArbitrageListener()
            );
            
            return Task.CompletedTask;
        }

        public void UpdateTriArbThreshold(decimal newThreshold)
        {
            if(newThreshold > 0)
            {
                _profitThreshold = newThreshold;
            }
        }

        public decimal GetTriangleThreshold()
        {
            return _profitThreshold;
        }

        //Calculate arb chances for triangle arb and pass it to the UI (?via SignalR?)
        public Task StartTriangleArbitrageListener()
        {
            while (true)
            {
                try
                {
                    foreach (var exchange in _exchanges)
                    {
                        CheckExchangeForTriangleArbitrage(exchange);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        public void CheckExchangeForTriangleArbitrage(IExchange exchange)
        {
            try {
                Orderbook finalMarket;
                decimal altAmount, alt2Amount, finalAmount, baseAmount;
                //Assuming we are testing BTC/ETH->ETH/XRP->XRP/BTC, startCurrency == BTC, middleCurrency == ETH, endCurrency == XRP
                string startCurrency, middleCurrency, endCurrency; //The currency that's bought/sold from in the first transaction
                var audInvested = 100;
                var orderbooks = exchange.Orderbooks;
            

                foreach (var market in orderbooks)
                {
                    //Loop every market with a matching currency except itself (this could start on the base or alt currency)
                    foreach (var market2 in orderbooks.Where(x => x.Pair != market.Pair && (x.AltCurrency == market.AltCurrency || x.BaseCurrency == market.AltCurrency || x.AltCurrency == market.BaseCurrency || x.BaseCurrency == market.BaseCurrency)))
                    {
                        //If the base/alt currency for the next market is the base currency, we need to bid (i.e. Buy for first trade)
                        if (market.BaseCurrency == market2.BaseCurrency || market.BaseCurrency == market2.AltCurrency)
                        {
                            baseAmount = ConvertAudToCrypto(orderbooks, market.AltCurrency, audInvested);

                            if(baseAmount == 0)
                            {
                                continue; //Asset prices not loaded yet
                            }

                            try
                            {
                                var bids = orderbooks.First(x => x.Pair == market.Pair).Bids;
                                if(bids.Count() == 0)
                                {
                                    continue;
                                }
                                altAmount = baseAmount * GetPriceQuote(bids, baseAmount);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                            startCurrency = market.AltCurrency;
                            middleCurrency = market.BaseCurrency;
                        }
                        else //Else we need to ask (i.e. Sell for first trade)
                        {
                            baseAmount = ConvertAudToCrypto(orderbooks, market.BaseCurrency, audInvested);
                            if (baseAmount == 0)
                            {
                                continue; //Asset prices not loaded yet
                            }

                            try
                            {
                                var asks = orderbooks.First(x => x.Pair == market.Pair).Asks;
                                if (asks.Count() == 0)
                                {
                                    continue; //Prices not loaded yet
                                }
                                altAmount = baseAmount / GetPriceQuote(asks, ConvertBaseToAlt(asks.First().Price, baseAmount)); //~3000 ETH from 100 BTC
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                            startCurrency = market.BaseCurrency;
                            middleCurrency = market.AltCurrency;
                        }

                        //If the alt bought in step 1 is now a base, use ask price
                        if (market2.BaseCurrency == middleCurrency)
                        {
                            endCurrency = market2.AltCurrency;
                            try
                            {
                                var asks = orderbooks.First(x => x.Pair == market2.Pair).Asks;
                                if (asks.Count() == 0)
                                {
                                    continue; //Prices not loaded yet
                                }
                                alt2Amount = altAmount / GetPriceQuote(asks, ConvertBaseToAlt(asks.First().Price, altAmount));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                        }
                        else //Otherwise it's the alt currency (i.e. we're selling to the new coin)
                        {
                            endCurrency = market2.BaseCurrency;
                            try
                            {
                                var bids = orderbooks.First(x => x.Pair == market2.Pair).Bids;
                                if(bids.Count() == 0)
                                {
                                    continue; //Not loaded yet
                                }
                                alt2Amount = altAmount * GetPriceQuote(bids, altAmount);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                        }
                        //Find the final market (i.e. the market that has the middle and end currencies)
                        finalMarket = orderbooks.FirstOrDefault(x => (x.BaseCurrency == startCurrency || x.AltCurrency == startCurrency) && (x.BaseCurrency == endCurrency || x.AltCurrency == endCurrency));

                        //If null, there's no pairs to finish the arb
                        if (finalMarket == null)
                        {
                            continue;
                        }

                        //If the base currency is the first currency, we need to sell (i.e. use bid)
                        if(finalMarket.BaseCurrency == startCurrency)
                        {
                            try
                            {
                                var bids = orderbooks.First(x => x.Pair == finalMarket.Pair).Bids;
                                if(bids.Count() == 0)
                                {
                                    continue; //Not loaded yet
                                }
                                finalAmount = alt2Amount * GetPriceQuote(bids, alt2Amount);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                        }
                        else //Else we buy (i.e. use ask)
                        {
                            try
                            {
                                var asks = orderbooks.First(x => x.Pair == finalMarket.Pair).Asks;
                                if (asks.Count() == 0)
                                {
                                    continue; //Prices not loaded yet
                                }
                                finalAmount = alt2Amount / GetPriceQuote(asks, ConvertBaseToAlt(asks.First().Price, alt2Amount));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                        }

                        decimal percentProfit = (finalAmount - baseAmount) / baseAmount * 100;
                        var result = new ArbitrageResult()
                        {
                            Exchanges = new List<string>() { exchange.Name },
                            Pairs = new List<Pair>() {
                                new Pair(market.BaseCurrency, market.AltCurrency),
                                new Pair(market2.BaseCurrency, market2.AltCurrency),
                                new Pair(finalMarket.BaseCurrency, finalMarket.AltCurrency)
                            },
                            Profit = percentProfit,
                            TransactionFee = exchange.Fee * 3,
                            InitialCurrency = startCurrency,
                            InitialLiquidity = baseAmount,
                            Type = ArbitrageType.Triangle
                        };
                        StoreTriangleResults(result, baseAmount, finalAmount);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //Calculate arb chances for normal arb
        public Task StartNormalArbitrageListener()
        {
            while (true)
            {
                try
                {
                    foreach (var exchange in _exchanges)
                    {
                        CheckExchangeForNormalArbitrage(exchange);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        public void CheckExchangeForNormalArbitrage(IExchange startExchange)
        {
            var orderbooks = startExchange.Orderbooks;
            decimal endAmountBought;
            decimal audInvested = 100;

            try
            {
                foreach (var startOrderbook in orderbooks) {
                    foreach (var exchange in _exchanges.Where(x => x.Name != startExchange.Name))
                    {
                        //Base->Alt->Base
                        var baseAmount = ConvertAudToCrypto(orderbooks, startOrderbook.BaseCurrency, audInvested);
                        if (baseAmount == 0)
                        {
                            continue; //Asset prices not loaded yet
                        }

                        var endOrderbook = exchange.Orderbooks.FirstOrDefault(x => x.BaseCurrency == startOrderbook.BaseCurrency && x.AltCurrency == startOrderbook.AltCurrency || x.BaseCurrency == startOrderbook.AltCurrency && x.AltCurrency == startOrderbook.BaseCurrency);
                        if (endOrderbook == null)
                        {
                            continue; //Other exchange doesn't have the pair
                        }

                        var asks = startOrderbook.Asks;
                        if (asks.Count() == 0)
                        {
                            continue;
                        }
                        var startAmountBought = baseAmount / GetPriceQuote(asks, ConvertBaseToAlt(asks.First().Price, baseAmount));

                        if (endOrderbook.BaseCurrency == startOrderbook.BaseCurrency)
                        {
                            var endBids = endOrderbook.Bids;
                            if (endBids.Count() == 0)
                            {
                                continue;
                            }
                            endAmountBought = startAmountBought * GetPriceQuote(endBids, startAmountBought);
                        }
                        else
                        {
                            var endAsks = endOrderbook.Asks;
                            if (endAsks.Count() == 0)
                            {
                                continue;
                            }
                            endAmountBought = startAmountBought / GetPriceQuote(endAsks, ConvertBaseToAlt(endAsks.First().Price, startAmountBought));
                        }

                        decimal percentProfit = (endAmountBought - baseAmount) / baseAmount * 100;

                        var result = new ArbitrageResult()
                        {
                            Exchanges = new List<string>() { startExchange.Name, exchange.Name },
                            Pairs = new List<Pair>() { new Pair(startOrderbook.BaseCurrency, startOrderbook.AltCurrency) },
                            Profit = percentProfit,
                            TransactionFee = startExchange.Fee + exchange.Fee,
                            InitialCurrency = startOrderbook.BaseCurrency,
                            InitialLiquidity = baseAmount,
                            Type = ArbitrageType.Normal
                        };
                        StoreNormalResults(result, baseAmount, endAmountBought);

                        //Alt->Base->Alt
                        baseAmount = ConvertAudToCrypto(orderbooks, startOrderbook.AltCurrency, audInvested);
                        if (baseAmount == 0)
                        {
                            continue; //Asset prices not loaded yet
                        }

                        var bids = startOrderbook.Bids;
                        if (bids.Count() == 0)
                        {
                            continue;
                        }
                        startAmountBought = baseAmount * GetPriceQuote(bids, baseAmount);

                        if (endOrderbook.BaseCurrency == startOrderbook.BaseCurrency)
                        {
                            var endAsks = endOrderbook.Asks;
                            if (endAsks.Count() == 0)
                            {
                                continue;
                            }
                            endAmountBought = startAmountBought / GetPriceQuote(endAsks, ConvertBaseToAlt(endAsks.First().Price, startAmountBought));
                        }
                        else
                        {
                            var endBids = endOrderbook.Bids;
                            if (endBids.Count() == 0)
                            {
                                continue;
                            }
                            endAmountBought = startAmountBought * GetPriceQuote(endBids, startAmountBought);
                        }

                        percentProfit = (endAmountBought - baseAmount) / baseAmount * 100;

                        result = new ArbitrageResult()
                        {
                            Exchanges = new List<string>() { startExchange.Name, exchange.Name },
                            Pairs = new List<Pair>() { new Pair(startOrderbook.AltCurrency, startOrderbook.BaseCurrency) },
                            Profit = percentProfit,
                            TransactionFee = startExchange.Fee + exchange.Fee,
                            InitialCurrency = startOrderbook.AltCurrency,
                            InitialLiquidity = baseAmount,
                            Type = ArbitrageType.Normal
                        };
                        StoreNormalResults(result, baseAmount, endAmountBought);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in CheckExchangeForNormalArbitrage (" + e.Message + ")");
            }
        }

        public void StoreTriangleResults(ArbitrageResult result, decimal baseAmount, decimal finalAmount)
        {
            if (bestTriangleProfit.Profit < result.Profit)
            {
                bestTriangleProfit = result;
            }
            if (worstTriangleProfit.Profit > result.Profit)
            {
                worstTriangleProfit = result;
            }

            var currentResult = triangleResults.FirstOrDefault(x => x.Exchanges == result.Exchanges && x.Pairs == result.Pairs);
            if (currentResult == null)
            {
                triangleResults.Add(result);
            }
            else
            {
                currentResult = result;
            }

            if(result.Profit > result.TransactionFee)
            {
                if (this.TradingEnabled && _exchanges.Where(x => result.Exchanges.Contains(x.Name)).All(y => y.IsAuthenticated))
                {
                    var @newTradeEvent = new ArbitrageFoundIntegrationEvent(result);
                    _eventBus.Publish(@newTradeEvent);
                }
                if(profitableTriangleResults.Count() == 0 || (profitableTriangleResults.Last().Exchanges != result.Exchanges && profitableTriangleResults.Last().Pairs != result.Pairs))
                {
                    profitableTriangleResults.Add(result);
                }
            }
        }

        public void StoreNormalResults(ArbitrageResult result, decimal baseAmount, decimal finalAmount)
        {
            if (bestNormalProfit.Profit < result.Profit)
            {
                bestNormalProfit = result;
            }
            if (worstNormalProfit.Profit > result.Profit)
            {
                worstNormalProfit = result;
            }

            var currentResult = normalResults.FirstOrDefault(x => x.Exchanges == result.Exchanges && x.Pairs == result.Pairs);
            if (currentResult == null)
            {
                normalResults.Add(result);
            }
            else
            {
                currentResult = result;
            }

            if (result.Profit > result.TransactionFee)
            {
                if (this.TradingEnabled && _exchanges.Where(x => result.Exchanges.Contains(x.Name)).All(y => y.IsAuthenticated))
                {
                    var @newTradeEvent = new ArbitrageFoundIntegrationEvent(result);
                    _eventBus.Publish(@newTradeEvent);
                }
                if (profitableNormalResults.Count() == 0 || (profitableNormalResults.Last().Exchanges != result.Exchanges && profitableNormalResults.Last().Pairs != result.Pairs))
                {
                    profitableNormalResults.Add(result);
                }
            }
        }

        //Converts a base currency to an alt currency at the market rate
        public decimal ConvertBaseToAlt(decimal price, decimal baseCurrencyAmount)
        {
            var altCurrencyAmount = baseCurrencyAmount / price;

            return altCurrencyAmount;
        }

        //Converts AUD to crypto at the market rate
        public decimal ConvertAudToCrypto(List<Orderbook> orderbooks, string asset, decimal audAmount)
        {
            try
            {
                decimal btcAudPrice = _exchanges.First(x => x.Name == "BtcMarkets").Orderbooks.First(x => x.Pair == "BTC/AUD").Asks.First().Price; //Use BtcMarkets as BTC/AUD price reference since they have the most volume
                decimal btcFromAud = audAmount / btcAudPrice;

                if (asset == "BTC")
                {
                    return btcFromAud;
                }

                var btcAsset = orderbooks.FirstOrDefault(x => x.BaseCurrency == "BTC" && x.AltCurrency == asset || x.AltCurrency == "BTC" && x.BaseCurrency == asset);
                if (btcAsset == null || btcAsset.Asks.Count() == 0)
                {
                    return 0; //Not populated yet
                }
                decimal btcAssetPrice = btcAsset.Asks.First().Price;
                decimal assetFromBtc = btcFromAud / btcAssetPrice;

                return assetFromBtc;
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong in ConvertAudToCrypto (" + e.Message + ")");
                return 0;
            }
        }

        //Note: Amount should be the amount in the alt, not the amount in the base currency
        public decimal GetPriceQuote(List<OrderbookOrder> orders, decimal amount)
        {
            try
            {
                decimal price = 0;

                decimal amountLeft = amount;
                int i = 0;

                //Loop until our order's filled or we run out of orderbook
                while (amountLeft > 0 && i < orders.Count())
                {
                    decimal altAmount = orders[i].Amount;
                    //if (pair.EndsWith("USDT"))
                    //{
                    //    altAmount *= orders[i].Price;
                    //}
                    decimal amountBought = altAmount;
                    if (altAmount > amountLeft) //Make sure we only partial fill the last of the order
                    {
                        amountBought = amountLeft;
                    }

                    //Keep track of how much is left
                    amountLeft -= amountBought;

                    //Track average price payed for the amount filled
                    price += (orders[i].Price / (amount / amountBought));

                    i++;
                }

                if (i > orders.Count())
                {
                    throw new Exception("Orderbook too thin, couldn't calculate price, requested " + amount + " when only " + orders.Sum(x => x.Amount) + " was available");
                }

                return price;
            }
            catch(Exception e)
            {
                throw new Exception("Something went wrong in GetPriceQuote (" + e.Message + ")");
            }
        }
    }
}
