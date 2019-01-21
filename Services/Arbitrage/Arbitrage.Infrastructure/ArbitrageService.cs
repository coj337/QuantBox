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

        decimal bestProfit = -100;
        int profitableCount = 0;
        //TODO FOR FUTURE COLIN: Make the bid/asks work down the orderbook instead of using the top
        public void CheckExchangeForTriangleArbitrage(string exchange)
        {
            foreach (var market in Markets[exchange])
            {
                //CheckMarketForTriangleArbitrage(exchange, market.Key);
                //Loop every market except itself with the alt currency except itself //TODO: Optimize by starting at i=0 and 
                foreach (var market2 in Markets[exchange].Where(x => x.Key != market.Key && x.Value.AltCurrency == market.Value.AltCurrency || x.Value.BaseCurrency == market.Value.AltCurrency))
                {
                    //TODO: Store all triangles so we can simply iterate through them instead of all this
                    MarketData finalMarket;

                    //TODO: Count fees in calculations
                    var baseAmount = 100; //Assume we have 100 of the starting coin (BaseCurrency)
                    var altAmount = baseAmount / market.Value.Asks.First().Price; //~3000 ETH or ~1126252 XRP from 100 BTC

                    decimal alt2Amount;
                    //If the alt bought in step 1 is still an alt, use bid price (e.g. we're selling to the new coin)
                    if (market2.Value.AltCurrency == market.Value.AltCurrency)
                    {
                        alt2Amount = altAmount * market2.Value.Bids.First().Price; //~2988 ETH from 1126252 XRP (i.e. BTC->XRP->ETH->BTC)
                        finalMarket = Markets[exchange].Values.FirstOrDefault(x => x.BaseCurrency == market.Value.BaseCurrency && x.AltCurrency == market2.Value.BaseCurrency);
                    }
                    else //Otherwise it's the base currency (e.g. we're buying to the new coin)
                    {
                        alt2Amount = altAmount / market2.Value.Asks.First().Price; //~1129420 XRP from 3000 ETH (i.e. BTC->ETH->XRP->BTC)
                        finalMarket = Markets[exchange].Values.FirstOrDefault(x => x.BaseCurrency == market.Value.BaseCurrency && x.AltCurrency == market2.Value.AltCurrency);
                    }

                    //If null, there's no pairs to go Base/Alt->(Alt/Alt2 || Alt2/Alt)->Alt2/Base
                    if (finalMarket == null)
                    {
                        continue;
                    }

                    //TODO: Will this sometimes be a buy instead?
                    var finalAmount = alt2Amount * finalMarket.Bids.First().Price; //~100.3376728 BTC from 1129420 XRP
                    decimal percentProfit = (finalAmount - baseAmount) / baseAmount * 100;

                    if (bestProfit < percentProfit) {
                        bestProfit = percentProfit;
                    }
                    if (finalAmount > baseAmount * _triProfitThreshold)
                    {
                        Console.WriteLine("Profit found above 0.5%");
                        profitableCount++;
                        /*ArbitrageResult arbResult = new ArbitrageResult()
                        {
                            Exchange = exchange,
                            Path = market.Value.BaseCurrency + "/" + market.Value.AltCurrency + " -> " + market2.Value.BaseCurrency + "/" + market2.Value.AltCurrency + " -> " + finalMarket.BaseCurrency + "/" + finalMarket.AltCurrency,
                            NetworkFee = 0,
                            Spread = percentProfit,
                            TimePerLoop = 0, //TODO: Count properly
                            TransactionFee = 0.1m * 3
                        };*/
                        //_arbitrageHub.Clients.All.ReceiveTriangleArbitrage(arbResult);
                    }
                }
            }
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
