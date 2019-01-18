using BuildingBlocks.EventBus.Abstractions;
using Market.Domain;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Market.API.Exchanges;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Market.API.Services
{
    public class MarketService
    {
        private readonly IConfiguration _configuration;

        private readonly List<IExchange> _supportedExchanges;

        public MarketService(IConfiguration configuration, IEventBus eventBus)
        {
            _configuration = configuration;
            _supportedExchanges = new List<IExchange>()
            {
                new Binance(eventBus)
            };

            foreach (var exchange in _supportedExchanges)
            {
                Task.Run(() =>
                {
                    exchange.StartPriceListener();
                });
            }
        }

        public List<IExchange> GetSupportedExchanges()
        {
            return _supportedExchanges;
        }

        public List<string> GetSupportedAssets()
        {
            List<string> assets = new List<string>();
            foreach (var exchange in _supportedExchanges)
            {
                assets.AddRange(exchange.Currencies.Select(x => x.Symbol));
            }
            return assets;
        }

        public List<MarketData> GetTickers()
        {
            List<MarketData> tickers = new List<MarketData>();
            foreach (var exchange in _supportedExchanges)
            {
                tickers.AddRange(exchange.Markets.Values);
            }
            return tickers;
        }
    }
}
