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
                    await Task.Delay(1000);
                    continue;
                }
            }
        }

        private decimal bestProfit = -100;
        private decimal worstProfit = 100;
        private readonly List<ArbitrageResult> profitableTransactions = new List<ArbitrageResult>();
        public void CheckExchangeForTriangleArbitrage(string exchange)
        {
            MarketData finalMarket;
            decimal altAmount, alt2Amount, finalAmount, baseAmount;
            var audInvested = 1000; //Assume we have 1 of the starting coin
            /*
             * Assuming we are testing BTC/ETH->ETH/XRP->XRP/BTC
             * startCurrency == BTC
             * middleCurrency == ETH
             * endCurrency == XRP
            */
            string startCurrency, middleCurrency, endCurrency; //The currency that's bought/sold from in the first transaction

            foreach (var market in Markets[exchange].Where(x =>/*SKIPPING BECAUSE BTCMARKETS BUG*/x.Key != "BTC/AUD"/**/))
            {

                //Loop every market with a matching currency except itself (this could start on the base or alt currency)
                foreach (var market2 in Markets[exchange].Where(x => /*SKIPPING BECAUSE BTCMARKETS BUG*/x.Key != "BTC/AUD" && /**/x.Key != market.Key && (x.Value.AltCurrency == market.Value.AltCurrency || x.Value.BaseCurrency == market.Value.AltCurrency || x.Value.AltCurrency == market.Value.BaseCurrency || x.Value.BaseCurrency == market.Value.BaseCurrency)))
                {
                    //If the base/alt currency for the next market is the base currency, we need to bid (i.e. Buy for first trade)
                    if (market.Value.BaseCurrency == market2.Value.BaseCurrency || market.Value.BaseCurrency == market2.Value.AltCurrency)
                    {
                        baseAmount = ConvertAudToCrypto(exchange, market.Value.AltCurrency, audInvested);
                        if(baseAmount == 0)
                        {
                            continue; //Asset prices not loaded yet
                        }

                        try
                        {
                            altAmount = baseAmount * GetPriceQuote(exchange, market.Key, OrderbookType.Bid, baseAmount);
                        }
                        catch(Exception e)
                        {
                            continue;
                        }
                        startCurrency = market.Value.AltCurrency;
                        middleCurrency = market.Value.BaseCurrency;
                    }
                    else //Else we need to ask (i.e. Sell for first trade)
                    {
                        baseAmount = ConvertAudToCrypto(exchange, market.Value.BaseCurrency, audInvested);
                        if (baseAmount == 0)
                        {
                            continue; //Asset prices not loaded yet
                        }

                        try
                        {
                            ///TODO: Is it enough to just transform baseAmount?
                            altAmount = baseAmount / GetPriceQuote(exchange, market.Key, OrderbookType.Ask, ConvertBaseToAlt(exchange, market.Value.BaseCurrency, market.Value.AltCurrency, baseAmount)); //~3000 ETH from 100 BTC
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                        startCurrency = market.Value.BaseCurrency;
                        middleCurrency = market.Value.AltCurrency;
                    }

                    //If the alt bought in step 1 is now a base, use ask price
                    if (market2.Value.BaseCurrency == middleCurrency)
                    {
                        endCurrency = market2.Value.AltCurrency;
                        try
                        {
                            alt2Amount = altAmount / GetPriceQuote(exchange, market2.Key, OrderbookType.Ask, ConvertBaseToAlt(exchange, market2.Value.BaseCurrency, market2.Value.AltCurrency, altAmount));
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                    else //Otherwise it's the alt currency (i.e. we're selling to the new coin)
                    {
                        endCurrency = market2.Value.BaseCurrency;
                        try
                        {
                            alt2Amount = altAmount * GetPriceQuote(exchange, market2.Key, OrderbookType.Bid, altAmount);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                    //Find the final market (i.e. the market that has the middle and end currencies)
                    finalMarket = Markets[exchange].Values.SingleOrDefault(x => /*SKIPPING BECAUSE BTCMARKETS BUG*/x.Pair != "BTC/AUD" && /**/(x.BaseCurrency == startCurrency || x.AltCurrency == startCurrency) && (x.BaseCurrency == endCurrency || x.AltCurrency == endCurrency));

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
                            finalAmount = alt2Amount * GetPriceQuote(exchange, finalMarket.Pair, OrderbookType.Bid, alt2Amount);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                    else //Else we buy (i.e. use ask)
                    {
                        try
                        { 
                            finalAmount = alt2Amount / GetPriceQuote(exchange, finalMarket.Pair, OrderbookType.Ask, ConvertBaseToAlt(exchange, finalMarket.BaseCurrency, finalMarket.AltCurrency, alt2Amount));
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }

                    decimal percentProfit = (finalAmount - baseAmount) / baseAmount * 100;
                    if (bestProfit < percentProfit) {
                        bestProfit = percentProfit;
                    }
                    if(worstProfit > percentProfit)
                    {
                        worstProfit = percentProfit;
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

        //Converts a base currency to an alt currency at the market rate
        public decimal ConvertBaseToAlt(string exchange, string baseCurrency, string altCurrency, decimal baseCurrencyAmount)
        {
            var price = Markets[exchange].First(x => x.Value.BaseCurrency == baseCurrency && x.Value.AltCurrency == altCurrency).Value.Asks.First().Price;

            var altCurrencyAmount = baseCurrencyAmount / price;

            return altCurrencyAmount;
        }

        //Converts AUD to crypto at the market rate
        public decimal ConvertAudToCrypto(string exchange, string asset, decimal audAmount)
        {
            try
            {
                decimal btcAudPrice = Markets["BtcMarkets"]["BTC/AUD"].Asks.First().Price; //Use BtcMarkets as BTC/AUD price reference since they have the most volume
                decimal btcFromAud = audAmount / btcAudPrice;

                decimal btcAssetPrice = Markets[exchange].FirstOrDefault(x => x.Value.BaseCurrency == "BTC" && x.Value.AltCurrency == asset || x.Value.AltCurrency == "BTC" && x.Value.BaseCurrency == asset).Value.Asks.First().Price;
                if (btcAssetPrice == 0)
                {
                    return 0;
                }
                decimal assetFromBtc = btcFromAud / btcAssetPrice;

                return assetFromBtc;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        //Note: Amount should be the amount in the alt, not the amount in the base currency
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
                price = price + (orders[i].Price / (amount / amountBought));

                i++;
            }

            if(i >= orders.Count())
            {
                throw new Exception("Orderbook too thin, couldn't calculate price for " + pair + " on " + exchange + ", requested " + amount + " when only " + orders.Sum(x => x.Amount) + " was available");
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
