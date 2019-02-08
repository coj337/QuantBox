using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinjarApiClient;
using ExchangeSharp;
using OrderType = ExchangeManager.Models.OrderType;

namespace ExchangeManager.Clients
{
    public class Coinjar : IExchange
    {
        private readonly CoinjarClient _client;
        public string Name => "Coinjar";
        public decimal Fee => 1m;
        public bool IsAuthenticated { get; private set; }
        public Dictionary<string, Orderbook> Orderbooks { get; private set; }
        public Dictionary<string, CurrencyData> Currencies { get; private set; }

        public Coinjar()
        {
            _client = new CoinjarClient();
            Orderbooks = new Dictionary<string, Orderbook>();
            Currencies = new Dictionary<string, CurrencyData>();
            IsAuthenticated = false;
        }

        public bool Authenticate(string publicKey, string privateKey)
        {
            throw new NotImplementedException();
            _client.SetCredentials(publicKey, privateKey);

            //Make sure auth didn't fail
            return false;

            return true;
        }

        //Coinjar is a market maker and doesn't have orderbooks (I think) so just return infinite depth
        public Task StartOrderbookListener()
        {
            var markets = _client.GetMarketsAsync().GetAwaiter().GetResult();

            foreach (var market in markets)
            {
                Orderbooks.Add(market.AltCurrency + "/" + market.BaseCurrency, 
                    new Orderbook()
                    {
                        Pair = market.Pair,
                        BaseCurrency = market.BaseCurrency,
                        AltCurrency = market.AltCurrency,
                        Asks = new List<OrderbookOrder> { new OrderbookOrder() { Price = market.Ask ?? 0, Amount = Decimal.MaxValue } },
                        Bids = new List<OrderbookOrder> { new OrderbookOrder() { Price = market.Bid ?? 0, Amount = Decimal.MaxValue } }
                    }
                );
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
                            var orderbook = Orderbooks[market.AltCurrency + "/" + market.BaseCurrency];
                            orderbook.Pair = market.Pair;
                            orderbook.BaseCurrency = market.BaseCurrency;
                            orderbook.AltCurrency = market.AltCurrency;
                            orderbook.Asks = new List<OrderbookOrder> { new OrderbookOrder() { Price = market.Ask ?? orderbook.Asks.First().Price, Amount = Decimal.MaxValue } }; //Sometimes the API returns null for no reason so just leave the price the same if it does
                            orderbook.Bids = new List<OrderbookOrder> { new OrderbookOrder() { Price = market.Bid ?? orderbook.Bids.First().Price, Amount = Decimal.MaxValue } }; 
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

        public Task<ExchangeOrderResult> CreateOrder(string pair, OrderSide side, OrderType type, decimal price, decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task<ExchangeOrderResult> SimulateOrder(string pair, OrderSide side, OrderType type, decimal price, decimal amount, double delaySeconds = 0)
        {
            throw new NotImplementedException();
        }
    }
}
