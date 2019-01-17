using BuildingBlocks.EventBus.Abstractions;
using Market.Domain;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Market.API.Exchanges;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

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
            return new List<string>() { "BTC", "ETH", "XRP", "XLM" }; //TODO: Enumerate the exchanges for this
        }
    }
}
