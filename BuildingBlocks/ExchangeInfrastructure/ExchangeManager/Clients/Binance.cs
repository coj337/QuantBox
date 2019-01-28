using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeManager.Clients
{
    public class Binance : IExchange
    {
        private readonly ExchangeBinanceAPI _client;

        public string Name => "Binance";
        public decimal Fee => 0.1m;
        public List<Orderbook> Orderbooks { get; private set; }
        public List<CurrencyData> Currencies { get; private set; }

        public Binance()
        {
            _client = new ExchangeBinanceAPI();
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
            catch(Exception)
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
                //Subscribe to ticker websockets
                var socket = _client.GetFullOrderBookWebSocket((orderbook) =>
                {
                    try
                    {
                        List<Order> bids = new List<Order>();
                        List<Order> asks = new List<Order>();

                        foreach (var bid in orderbook.Bids.Values)
                        {
                            bids.Add(new Order() { Price = bid.Price, Amount = bid.Amount });
                        }
                        foreach (var ask in orderbook.Asks.Values)
                        {
                            asks.Add(new Order() { Price = ask.Price, Amount = ask.Amount });
                        }

                        var thisOrderbook = Orderbooks.First(x => x.Pair == orderbook.MarketSymbol);
                        thisOrderbook.Bids = bids;
                        thisOrderbook.Asks = asks;
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                    } //Don't let an exception kill our socket
                });
            });

            return Task.CompletedTask;
        }
    }
}
