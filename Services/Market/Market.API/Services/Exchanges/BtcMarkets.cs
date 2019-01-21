using BuildingBlocks.EventBus.Abstractions;
using Market.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BtcMarketsApiClient;
using System.Threading;
using Market.API.IntegrationEvents.Events;

namespace Market.API.Services.Exchanges
{
    public class BtcMarkets : IExchange
    {
        private readonly IEventBus _eventBus;
        private readonly BtcMarketsClient _client;
        
        public string Name => "BtcMarkets";
        public Dictionary<string, MarketData> Markets { get; private set; }
        public List<CurrencyData> Currencies { get; private set; }

        public BtcMarkets(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _client = new BtcMarketsClient();
            Markets = new Dictionary<string, MarketData>();
            Currencies = new List<CurrencyData>();
        }

        public bool Authenticate(string publicKey, string privateKey)
        {
            _client.SetCredentials(publicKey, privateKey);

            //Make sure auth didn't fail
            if (!IsAuthenticated())
            {
                return false;
            }

            return true;
        }

        public bool IsAuthenticated()
        {
            return !_client.RetrieveAccountBalance().Contains("Authentication failed");
        }

        public Task StartPriceListener()
        {
            //throw new NotImplementedException();
            //Ignore for now
            return Task.CompletedTask;
        }

        public Task StartOrderbookListener()
        {
            /////TEMP WHILE WE AREN'T USING THE OTHER SOCKET
            var markets = _client.GetMarkets();

            //Subscribe to ticker websockets
            //TODO: Implement sockets in client
            while (true)
            {
                foreach (var pair in markets)
                {
                    var orderbook = _client.GetOrderBook(pair);

                    List<Order> bids = new List<Order>();
                    List<Order> asks = new List<Order>();

                    foreach (var bid in orderbook.bids)
                    {
                        bids.Add(new Order() { Price = bid[0], Amount = bid[1] });
                    }
                    foreach (var ask in orderbook.asks)
                    {
                        asks.Add(new Order() { Price = ask[0], Amount = ask[1] });
                    }

                    //Send integration event for other services
                    var @event = new OrderbookUpdatedIntegrationEvent(this.Name, pair, orderbook.currency, orderbook.instrument, bids, asks);
                    _eventBus.Publish(@event);
                }

                Thread.Sleep(1000);
            }
        }
    }
}
