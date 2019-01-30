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

        //Coinjar is a market maker and doesn't have orderbooks (I think) so just return infinite depth
        public Task StartOrderbookListener()
        {
            var markets = _client.GetMarketsAsync().GetAwaiter().GetResult();

            foreach (var market in markets)
            {
                Orderbooks.Add(new Orderbook()
                {
                    Pair = market.Pair,
                    BaseCurrency = market.BaseCurrency,
                    AltCurrency = market.AltCurrency,
                    Asks = new List<Order> { new Order() { Price = market.Ask, Amount = Decimal.MaxValue } },
                    Bids = new List<Order> { new Order() { Price = market.Bid, Amount = Decimal.MaxValue } }
                });
            }

            Task.Run(async () =>
            {
                //Get price forever
                while (true)
                {
                    try
                    {
                        markets = await _client.GetMarketsAsync();

                        foreach (var market in markets)
                        {
                            var orderbook = Orderbooks.First(x => x.Pair == market.Pair);
                            orderbook.Pair = market.Pair;
                            orderbook.BaseCurrency = market.BaseCurrency;
                            orderbook.AltCurrency = market.AltCurrency;
                            orderbook.Asks = new List<Order> { new Order() { Price = market.Ask, Amount = Decimal.MaxValue } };
                            orderbook.Bids = new List<Order> { new Order() { Price = market.Bid, Amount = Decimal.MaxValue } };
                        }

                        await Task.Delay(1000);
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
