using ExchangeManager.Clients;
using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Services
{
    public class PriceService
    {
        private readonly List<IExchange> _exchanges = new List<IExchange>()
        {
            new Binance(),
            new KuCoin(),
            new BtcMarkets(),
            new Coinjar()
        };

        public PriceService()
        {
            foreach (var exchange in _exchanges)
            {
                exchange.StartOrderbookListener();
            }
        }

        public List<IExchange> GetExchanges()
        {
            return _exchanges;
        }
    }
}
