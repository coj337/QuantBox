using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeManager.Clients
{
    public class KuCoin : IExchange
    {
        private readonly ExchangeKucoinAPI _client;

        public string Name => "KuCoin";
        public decimal Fee => 0.1m;
        public List<Orderbook> Orderbooks { get; private set; }
        public List<CurrencyData> Currencies { get; private set; }

        public KuCoin()
        {
            _client = new ExchangeKucoinAPI();
            Orderbooks = new List<Orderbook>();
            Currencies = new List<CurrencyData>();
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

            return true;
        }

        public bool IsAuthenticated()
        {
            try
            {
                _client.GetDepositHistoryAsync("BTC").GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task StartOrderbookListener()
        {
            var markets = _client.GetMarketSymbolsMetadataAsync().GetAwaiter().GetResult();

            foreach (var market in markets)
            {
                Orderbooks.Add(new Orderbook()
                {
                    Pair = market.MarketSymbol,
                    BaseCurrency = market.QuoteCurrency,
                    AltCurrency = market.BaseCurrency,
                    Asks = new List<Order>(),
                    Bids = new List<Order>()
                });
            }

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var market in markets)
                        {
                            List<Order> bids = new List<Order>();
                            List<Order> asks = new List<Order>();

                            var orderbook = _client.GetOrderBookAsync(market.MarketSymbol).GetAwaiter().GetResult();

                            foreach (var bid in orderbook.Bids.Values)
                            {
                                bids.Add(new Order() { Price = bid.Price, Amount = bid.Amount });
                            }
                            foreach (var ask in orderbook.Asks.Values)
                            {
                                asks.Add(new Order() { Price = ask.Price, Amount = ask.Amount });
                            }

                            var thisOrderbook = Orderbooks.First(x => x.Pair == market.MarketSymbol);
                            thisOrderbook.Bids = bids;
                            thisOrderbook.Asks = asks;
                        }

                        Task.Delay(1000).Wait();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in KuCoin orderbook listener (" + e.Message + ")");
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
}
