using ExchangeManager.Helpers;
using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderType = ExchangeManager.Models.OrderType;

namespace ExchangeManager.Clients
{
    public class KuCoin : IExchange
    {
        private readonly ExchangeKucoinAPI _client;

        public string Name => "KuCoin";
        public decimal Fee => 0.1m;
        public bool IsAuthenticated { get; private set; }
        public Dictionary<string, Orderbook> Orderbooks { get; private set; }
        public Dictionary<string, CurrencyData> Currencies { get; private set; }

        public KuCoin()
        {
            ExchangeAPI.UseDefaultMethodCachePolicy = false;

            _client = new ExchangeKucoinAPI();
            Orderbooks = new Dictionary<string, Orderbook>();
            Currencies = new Dictionary<string, CurrencyData>();
            IsAuthenticated = false;
        }

        public bool Authenticate(string publicKey, string privateKey)
        {
            _client.PublicApiKey = publicKey.ToSecureString();
            _client.PrivateApiKey = privateKey.ToSecureString();

            //Make sure auth didn't fail
            try
            {
                _client.GetDepositHistoryAsync("BTC").GetAwaiter().GetResult();
                this.IsAuthenticated = true;
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
                Orderbooks.Add(market.BaseCurrency + "/" + market.QuoteCurrency, 
                    new Orderbook()
                    {
                        Pair = market.MarketSymbol,
                        BaseCurrency = market.QuoteCurrency,
                        AltCurrency = market.BaseCurrency,
                        Asks = new List<OrderbookOrder>(),
                        Bids = new List<OrderbookOrder>()
                    }
                );
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var market in markets)
                        {
                            List<OrderbookOrder> bids = new List<OrderbookOrder>();
                            List<OrderbookOrder> asks = new List<OrderbookOrder>();

                            var orderbook = await _client.GetOrderBookAsync(market.MarketSymbol);

                            bids.AddRange(orderbook.Bids.Values.Select(x => new OrderbookOrder() { Price = x.Price, Amount = x.Amount }));
                            asks.AddRange(orderbook.Asks.Values.Select(x => new OrderbookOrder() { Price = x.Price, Amount = x.Amount }));

                            var thisOrderbook = Orderbooks[market.BaseCurrency + "/" + market.QuoteCurrency];
                            thisOrderbook.Bids = bids;
                            thisOrderbook.Asks = asks;
                        }

                        await Task.Delay(1000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in KuCoin orderbook listener (" + e.Message + ")");
                    }
                }
            });

            return Task.CompletedTask;
        }

        public async Task<ExchangeOrderResult> CreateOrder(string pair, OrderSide side, OrderType type, decimal price, decimal amount)
        {
            var resp = await _client.PlaceOrderAsync(new ExchangeOrderRequest()
            {
                MarketSymbol = pair,
                Amount = amount,
                Price = price,
                IsBuy = side == OrderSide.Buy,
                OrderType = type.ToExSharpType(),
                ShouldRoundAmount = true
            });

            return resp;
        }
    }
}
