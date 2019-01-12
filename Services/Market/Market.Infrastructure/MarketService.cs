using Market.Domain;
using Market.Domain.Exchanges;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Market.Infrastructure
{
    public class MarketService
    {
        private readonly IConfiguration _configuration;

        List<IExchange> _supportedExchanges = new List<IExchange>()
        {
            new Binance()
        };

        public MarketService(IConfiguration configuration)
        {
            _configuration = configuration;

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
            }
        }


    }
}
