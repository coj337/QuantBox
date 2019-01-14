using BuildingBlocks.EventBus.Abstractions;
using Market.Domain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Market.API.Exchanges;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Market.API.Services
{
    public class MarketService : IHostedService
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
                string publicKey = configuration[exchange.Name + ":PublicKey"];
                string privateKey = configuration[exchange.Name + ":PrivateKey"];

                if(string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
                {
                    throw new Exception("Missing a key for " + exchange.Name);
                }

                if(!exchange.Authenticate(publicKey, privateKey))
                {
                    throw new Exception("Authentication failed for " + exchange.Name);
                }

                exchange.StartPriceListener();
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
