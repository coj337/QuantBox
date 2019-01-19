using Arbitrage.Domain;
using Arbitrage.Domain.ExchangeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrage.Infrastructure
{
    public class ArbitrageService
    {
        public Dictionary<string, Dictionary<string, MarketData>> Markets { get; set; }

        public ArbitrageService()
        {
            Markets = new Dictionary<string, Dictionary<string, MarketData>>();
            Task.Run(() =>
                StartTriangleArbitrageListener()
            );
        }

        public void UpdatePrice(ExchangeData marketData)
        {
            if (!Markets.ContainsKey(marketData.Exchange))
            {
                Markets.Add(marketData.Exchange, new Dictionary<string, MarketData>());
            }
            if (!Markets[marketData.Exchange].ContainsKey(marketData.Pair))
            {
                Markets[marketData.Exchange].Add(marketData.Pair, new MarketData());
            }

            Markets[marketData.Exchange][marketData.Pair].BaseCurrency = marketData.BaseCurrency;
            Markets[marketData.Exchange][marketData.Pair].AltCurrency = marketData.AltCurrency;
            Markets[marketData.Exchange][marketData.Pair].Ask = marketData.Data.Ask;
            Markets[marketData.Exchange][marketData.Pair].Bid = marketData.Data.Bid;
        }

        //Calculate arb chances for triangle arb and pass it to the UI via SignalR
        public void StartTriangleArbitrageListener()
        {
            while (true)
            {
                try
                {
                    foreach (var exchange in Markets.ToList())
                    {
                        foreach (var market in Markets[exchange.Key].ToList())
                        {
                            //Loop every market except itself with the alt currency except itself //TODO: Optimize by starting at i=0 and 
                            foreach (var market2 in Markets[exchange.Key].ToList().Where(x => x.Key != market.Key && x.Value.AltCurrency == market.Value.AltCurrency || x.Value.BaseCurrency == market.Value.AltCurrency))
                            {
                                //TODO: Store all triangles so we can simply iterate through them instead of all this
                                MarketData finalMarket;

                                //TODO: Count fees in calculations
                                var baseAmount = 100; //Assume we have 100 of the starting coin (BaseCurrency)
                                var altAmount = baseAmount / market.Value.Ask; //~3000 ETH or ~1126252 XRP from 100 BTC

                                decimal alt2Amount;
                                //If the alt bought in step 1 is still an alt, use bid price (e.g. we're selling to the new coin)
                                if (market2.Value.AltCurrency == market.Value.AltCurrency)
                                {
                                    alt2Amount = altAmount * market2.Value.Bid; //~2988 ETH from 1126252 XRP (i.e. BTC->XRP->ETH->BTC)
                                    finalMarket = Markets[exchange.Key].Values.ToList().FirstOrDefault(x => x.BaseCurrency == market.Value.BaseCurrency && x.AltCurrency == market2.Value.BaseCurrency);
                                }
                                else //Otherwise it's the base currency (e.g. we're buying to the new coin)
                                {
                                    alt2Amount = altAmount / market2.Value.Ask; //~1129420 XRP from 3000 ETH (i.e. BTC->ETH->XRP->BTC)
                                    finalMarket = Markets[exchange.Key].Values.ToList().FirstOrDefault(x => x.BaseCurrency == market.Value.BaseCurrency && x.AltCurrency == market2.Value.AltCurrency);
                                }

                                //If null, there's no pairs to go Base/Alt->(Alt/Alt2 || Alt2/Alt)->Alt2/Base
                                if (finalMarket == null)
                                {
                                    continue;
                                }

                                //TODO: Will this sometimes be a buy instead?
                                var finalAmount = alt2Amount * finalMarket.Bid; //~100.3376728 BTC from 1129420 XRP
                                Console.WriteLine("Started with " + baseAmount + " " + market.Value.BaseCurrency + " and ended up with " + finalAmount + " " + market.Value.BaseCurrency + " (" + ((finalAmount - baseAmount) / baseAmount * 100).ToString("N4") + "% Change)");

                                if(finalAmount > baseAmount * 1.005m)
                                {
                                    Console.WriteLine("Profit found above 0.5%");
                                }
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    continue;
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
