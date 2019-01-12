using Arbitrage.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Infrastructure
{
    public class ArbitrageService
    {
        private readonly List<Exchange> _exchanges;

        public ArbitrageService()
        {
            _exchanges = new List<Exchange>() //TODO: Get from market service
            {
                new Exchange()
                {
                    Name = "Binance",
                    TradeFee = 0.2m,
                    Pairs = new List<Pair>()
                    {
                        new Pair()
                        {
                            BaseCurrency = new Currency()
                            {
                                symbol = "BTC"
                            },
                            AltCurrency = new Currency()
                            {
                                symbol = "ETH"
                            }
                        }
                    }
                }
            };

            Task.Run(() =>
                StartArbitrageListener()
            );
        }

        //Calculate arb chances for everything and pass it to the UI via SignalR
        public void StartArbitrageListener()
        {
            foreach(var exchange in _exchanges)
            {

            }
        }
    }
}
