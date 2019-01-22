using Arbitrage.Domain;
using Arbitrage.Domain.ArbitrageResultAggregate;
using Arbitrage.Domain.ExchangeAggregate;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrage.Infrastructure
{
    public class ArbitrageService
    {
        private readonly IHubContext<ArbitrageHub, IArbitrageHub> _arbitrageHub;
        private decimal _triProfitThreshold = 1.005m; //0.5% profit default
        public ConcurrentDictionary<string, ConcurrentDictionary<string, MarketData>> Markets { get; set; }

        public ArbitrageService(IHubContext<ArbitrageHub, IArbitrageHub> arbitrageHub)
        {
            _arbitrageHub = arbitrageHub;
            Markets = new ConcurrentDictionary<string, ConcurrentDictionary<string, MarketData>>();
            Task.Run(() =>
                StartTriangleArbitrageListener()
            );
        }

        public void UpdateTriArbThreshold(decimal newThreshold)
        {
            if(newThreshold > 0)
            {
                _triProfitThreshold = newThreshold;
            }
        }

        public void UpdatePrice(ExchangeData marketData)
        {

            if (!Markets.ContainsKey(marketData.Exchange))
            {
                bool success = Markets.TryAdd(marketData.Exchange, new ConcurrentDictionary<string, MarketData>());
                if (!success) //TODO: Handle this better
                    return; //Dont update anything if it fails
            }
            if (!Markets[marketData.Exchange].ContainsKey(marketData.Data.Pair))
            {
                bool success = Markets[marketData.Exchange].TryAdd(marketData.Data.Pair, new MarketData());
                if (!success) //TODO: Handle this better
                    return; //Dont update anything if it fails
            }

            Markets[marketData.Exchange][marketData.Data.Pair].Pair = marketData.Data.Pair;
            Markets[marketData.Exchange][marketData.Data.Pair].BaseCurrency = marketData.Data.BaseCurrency;
            Markets[marketData.Exchange][marketData.Data.Pair].AltCurrency = marketData.Data.AltCurrency;
            Markets[marketData.Exchange][marketData.Data.Pair].Asks = marketData.Data.Asks;
            Markets[marketData.Exchange][marketData.Data.Pair].Bids = marketData.Data.Bids;
        }

        //Calculate arb chances for triangle arb and pass it to the UI via SignalR
        public async Task StartTriangleArbitrageListener()
        {
            while (true)
            {
                try
                {
                    foreach (var exchange in Markets)
                    {
                        CheckExchangeForTriangleArbitrage(exchange.Key);
                    }

                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        private decimal bestProfit = -100;
        private readonly List<ArbitrageResult> profitableTransactions = new List<ArbitrageResult>();
        public void CheckExchangeForTriangleArbitrage(string exchange)
        {
            foreach (var market in Markets[exchange])
            {
                //CheckMarketForTriangleArbitrage(exchange, market.Key);
                //Loop every market except itself with the alt currency //TODO: Optimize by starting at i=0 and 
                foreach (var market2 in Markets[exchange].Where(x => x.Key != market.Key && x.Value.AltCurrency == market.Value.AltCurrency || x.Value.BaseCurrency == market.Value.AltCurrency))
                {
                    //TODO: Store all triangles so we can simply iterate through them instead of all this
                    MarketData finalMarket;

                    //TODO: Count fees in calculations
                    var baseAmount = 1; //Assume we have 100 of the starting coin (BaseCurrency)
                    var altAmount = baseAmount / GetPriceQuote(exchange, market.Key, OrderbookType.Ask, baseAmount); //~3000 ETH or ~1126252 XRP from 100 BTC

                    decimal alt2Amount;
                    //If the alt bought in step 1 is still an alt, use bid price (e.g. we're selling to the new coin)
                    if (market2.Value.AltCurrency == market.Value.AltCurrency)
                    {
                        alt2Amount = altAmount * GetPriceQuote(exchange, market2.Key, OrderbookType.Bid, altAmount); //~2988 ETH from 1126252 XRP (i.e. BTC->XRP->ETH->BTC)
                        finalMarket = Markets[exchange].Values.FirstOrDefault(x => x.BaseCurrency == market.Value.BaseCurrency && x.AltCurrency == market2.Value.BaseCurrency);
                    }
                    else //Otherwise it's the base currency (e.g. we're buying to the new coin)
                    {
                        alt2Amount = altAmount / GetPriceQuote(exchange, market2.Key, OrderbookType.Ask, altAmount); //~1129420 XRP from 3000 ETH (i.e. BTC->ETH->XRP->BTC)
                        finalMarket = Markets[exchange].Values.FirstOrDefault(x => x.BaseCurrency == market.Value.BaseCurrency && x.AltCurrency == market2.Value.AltCurrency);
                    }

                    //If null, there's no pairs to go Base/Alt->(Alt/Alt2 || Alt2/Alt)->Alt2/Base
                    if (finalMarket == null)
                    {
                        continue;
                    }

                    //TODO: Will this sometimes be a buy instead?
                    var finalAmount = alt2Amount * GetPriceQuote(exchange, finalMarket.Pair, OrderbookType.Bid, altAmount); //~100.3376728 BTC from 1129420 XRP
                    decimal percentProfit = (finalAmount - baseAmount) / baseAmount * 100;

                    if (bestProfit < percentProfit) {
                        bestProfit = percentProfit;
                    }
                    if (finalAmount > baseAmount * _triProfitThreshold)
                    {
                        Console.WriteLine("Profit found above 0.5%");
                        ArbitrageResult arbResult = new ArbitrageResult()
                        {
                            Exchange = exchange,
                            Path = market.Value.BaseCurrency + "/" + market.Value.AltCurrency + " -> " + market2.Value.BaseCurrency + "/" + market2.Value.AltCurrency + " -> " + finalMarket.BaseCurrency + "/" + finalMarket.AltCurrency,
                            NetworkFee = 0,
                            Spread = percentProfit,
                            TimePerLoop = 0, //TODO: Count properly
                        };
                        if(exchange == "Binance")
                        {
                            arbResult.TransactionFee = 0.1m * 3;
                        }
                        else if(exchange == "BtcMarkets")
                        {
                            arbResult.TransactionFee = 0.22m * 3;
                        }
                        profitableTransactions.Add(arbResult);

                        //_arbitrageHub.Clients.All.ReceiveTriangleArbitrage(arbResult);
                    }
                    var bCount = profitableTransactions.Count(x => x.Exchange == "Binance");
                    var btcmCount = profitableTransactions.Count(x => x.Exchange == "BtcMarkets");
                }
            }
        }

        public decimal GetPriceQuote(string exchange, string pair, OrderbookType type, decimal amount)
        {
            decimal price = 0;
            List<Order> orders;

            if (type == OrderbookType.Ask)
            {
                orders = Markets[exchange][pair].Asks;
            }
            else
            {
                orders = Markets[exchange][pair].Bids;
            }

            decimal amountLeft = amount;
            int i = 0;

            //Loop until our order's filled or we run out of orderbook
            while (amountLeft > 0 && i < orders.Count())
            {
                decimal amountBought = orders[i].Amount;
                if (orders[i].Amount > amountLeft) //Make sure we only partial fill the last of the order
                {
                    amountBought = amountLeft;
                }

                //Keep track of how much is left
                amountLeft -= amountBought;

                //Track average price payed for the amount filled
                price = price + (orders[i].Price / (amount / amountBought));

                i++;
            }

            return price;
        }

        public enum OrderbookType
        {
            Bid,
            Ask
        }

        //Calculate arb chances for normal arb and pass it to the UI via SignalR
        public void StartNormalArbitrageListener()
        {
            foreach (var exchange in Markets)
            {
                foreach (var market in Markets[exchange.Key])
                {
                    //foreach (var market2 in Markets[exchange.Key].Where(x => x != market)){
                        //TODO: Do math
                    //}
                }
            }
        }
    }
}
