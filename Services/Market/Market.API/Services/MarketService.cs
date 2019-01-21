using BuildingBlocks.EventBus.Abstractions;
using Market.Domain;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Market.API.Exchanges;
using System.Threading.Tasks;
using System.Linq;
using System;
using Market.API.Infrastructure;

namespace Market.API.Services
{
    public class MarketService
    {
        private readonly MarketContext _context;

        private readonly List<IExchange> _supportedExchanges;

        public MarketService(IEventBus eventBus, MarketContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

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

                Task.Run(() =>
                {
                    exchange.StartOrderbookListener();
                });
            }
        }

        public List<ExchangeConfig> GetSettings()
        {
            List<ExchangeConfig> configs = new List<ExchangeConfig>();
            foreach(var exchange in _supportedExchanges)
            {
                ExchangeConfig config = _context.ExchangeConfigs.FirstOrDefault(x => x.Name == exchange.Name);
                if (config != null)
                {
                    config.PrivateKey = ""; //Scrub the private key, we don't want anything caching/intercepting it
                    configs.Add(config);
                }
                else
                {
                    configs.Add(new ExchangeConfig() { Name = exchange.Name, PublicKey = "" });
                }
            }
            return configs;
        }

        public void SaveSettings(ExchangeConfig config)
        {
            var existingConfig = _context.ExchangeConfigs.FirstOrDefault(x => x.Name == config.Name);
            if (existingConfig != null)
            {
                existingConfig.PublicKey = config.PublicKey;
                existingConfig.PrivateKey = config.PrivateKey;
                _context.ExchangeConfigs.Update(existingConfig);
            }
            else
            {
                _context.Add(config);
            }
            _context.SaveChanges();
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
