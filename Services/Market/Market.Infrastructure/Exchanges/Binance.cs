using ExchangeSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.Domain.Exchanges
{
    public class Binance : IExchange
    {
        private ExchangeBinanceAPI _client;

        public string Name { get => "Binance"; }
        public List<Market> Markets { get; private set; }

        //protected IReadOnlyCollection<KeyValuePair<string, ExchangeTicker>> Tickers { get; private set; }

        public Binance()
        {
            _client = new ExchangeBinanceAPI();
        }

        public bool Authenticate(string publicKey, string privateKey)
        {
            _client.PublicApiKey = publicKey.ToSecureString();
            _client.PrivateApiKey = privateKey.ToSecureString();

            //Make sure auth didn't fail
            if (!IsAuthenticated())
            {
                return false;
            }

            Task.Run(() =>
                StartPriceListener()
            );

            return true;
        }

        public bool IsAuthenticated()
        {
            return _client.GetFeesAync().IsCompletedSuccessfully;
        }

        public async Task StartPriceListener()
        {
            //Get all pairs before starting listener
            foreach(var market in await _client.GetSymbolsMetadataAsync())
            {
                Markets.Add(new Market(market.BaseCurrency, market.MarketCurrency));
            }

            //Subscribe to ticker websockets
            using (var socket = _client.GetTickersWebSocket((tickers) =>
            {
                for(var i = 0; i < Markets.Count(); i++)
                {
                    var foundTicker = tickers.FirstOrDefault(x => x.Key == ""); //TODO: Find key format and check next bit
                    var pair = foundTicker.Key.Split("/");

                    //If market exists, update it
                    if (foundTicker != null)
                    {
                        Markets.FirstOrDefault(x => x.BaseCurrency == pair[0] && x.AltCurrency == pair[1]).Bid = foundTicker.Value.Bid;
                        //TODO: The rest of them
                    }
                }
            }))
            {
                Console.ReadLine(); //TODO: Keep it open more gracefully
            }

            throw new NotImplementedException();
        }
    }
}
