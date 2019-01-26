using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinjarApiClient;

namespace ExchangeManager.Clients
{
    public class Coinjar : IExchange
    {
        private readonly CoinjarClient _client;
        public string Name => "Coinjar";
        public decimal Fee => 1m;
        public List<Orderbook> Orderbooks { get; private set; }
        public List<CurrencyData> Currencies { get; private set; }

        public Coinjar()
        {
            _client = new CoinjarClient();
            Orderbooks = new List<Orderbook>();
            Currencies = new List<CurrencyData>();
        }

        public bool Authenticate(string publicKey, string privateKey)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
            return _client.GetAccountBalance() != null;
        }

        //Coinjar is a market maker and doesn't have orderbooks (I think)
        public Task StartOrderbookListener()
        {
            throw new NotImplementedException();
            var markets = _client.GetMarkets();
            /*
            foreach (var market in markets)
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
                            if (orderbook == null)
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
            */
            return Task.CompletedTask;
        }
    }
}
