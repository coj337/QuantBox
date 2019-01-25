using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BtcMarketsApiClient;
using System.Threading;
using ExchangeManager.Models;

namespace ExchangeManager.Clients
{
    public class BtcMarkets : IExchange
    {
        private readonly BtcMarketsClient _client;
        
        public string Name => "BtcMarkets";
        public decimal Fee => 0.22m;
        public List<Orderbook> Orderbooks { get; private set; }
        public List<CurrencyData> Currencies { get; private set; }

        public BtcMarkets()
        {
            _client = new BtcMarketsClient();
            Orderbooks = new List<Orderbook>();
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

        public Task StartOrderbookListener()
        {
            var markets = _client.GetMarkets();

            foreach(var market in markets)
            {
                var orderbook = _client.GetOrderBook(market.Pair);
                Orderbooks.Add(new Orderbook()
                {
                    Pair = market.Pair,
                    BaseCurrency = market.Currency,
                    AltCurrency = market.Instrument,
                    Asks = orderbook.asks.Select(ask => new Order() { Price = ask[0], Amount = ask[1] }).ToList(),
                    Bids = orderbook.bids.Select(bid => new Order() { Price = bid[0], Amount = bid[1] }).ToList()
                });
            }

            Task.Run(() =>
            {
                //Subscribe to ticker websockets
                //TODO: Implement sockets in client
                while (true)
                {
                    try
                    {
                        foreach (var market in markets)
                        {
                            var orderbook = _client.GetOrderBook(market.Pair);
                            if(orderbook == null)
                            {
                                continue; //It breaks sometimes so just skip the pair this loop
                            }

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

                            var thisOrderbook = Orderbooks.First(x => x.Pair == market.Pair);
                            thisOrderbook.Bids = bids;
                            thisOrderbook.Asks = asks;
                        }

                        Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in BtcMarkets orderbook listener (" + e.Message + ")");
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
}
