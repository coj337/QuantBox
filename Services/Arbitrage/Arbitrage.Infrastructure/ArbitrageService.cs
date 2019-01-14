using Arbitrage.Domain;
using Arbitrage.Domain.ExchangeAggregate;
using System;
using System.Collections.Generic;
using System.Text;
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
                StartArbitrageListener()
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
            Markets[marketData.Exchange][marketData.Pair].Ask = marketData.Data.Ask;
            Markets[marketData.Exchange][marketData.Pair].Bid = marketData.Data.Bid;
        }

        //Calculate arb chances for everything and pass it to the UI via SignalR
        public void StartArbitrageListener()
        {
            //TODO: Do arb things
        }
    }
}
