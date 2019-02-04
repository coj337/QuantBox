using ExchangeManager.Helpers;
using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OrderType = ExchangeManager.Models.OrderType;

namespace ExchangeManager.Clients
{
    public class Binance : IExchange
    {
        private readonly ExchangeBinanceAPI _client;

        public string Name => "Binance";
        public decimal Fee => 0.1m;
        public bool IsAuthenticated { get; private set; }
        public Dictionary<string, Orderbook> Orderbooks { get; private set; }
        public Dictionary<string, CurrencyData> Currencies { get; private set; }

        public Binance()
        {
            ExchangeAPI.UseDefaultMethodCachePolicy = false;

            _client = new ExchangeBinanceAPI();
            Orderbooks = new Dictionary<string, Orderbook>();
            Currencies = new Dictionary<string, CurrencyData>();
            this.IsAuthenticated = false; //TODO: Pull from a DB for this
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
                Orderbooks.Add(market.BaseCurrency + "/" + market.QuoteCurrency, new Orderbook()
                {
                    Pair = market.MarketSymbol,
                    BaseCurrency = market.QuoteCurrency,
                    AltCurrency = market.BaseCurrency,
                    Asks = new List<OrderbookOrder>(),
                    Bids = new List<OrderbookOrder>()
                });
            }

            Task.Run(() =>
            {
                //Subscribe to ticker websockets
                var socket = _client.GetFullOrderBookWebSocket((orderbook) =>
                {
                    try
                    {
                        List<OrderbookOrder> bids = new List<OrderbookOrder>();
                        List<OrderbookOrder> asks = new List<OrderbookOrder>();

                        foreach (var bid in orderbook.Bids.Values)
                        {
                            bids.Add(new OrderbookOrder() { Price = bid.Price, Amount = bid.Amount });
                        }
                        foreach (var ask in orderbook.Asks.Values)
                        {
                            asks.Add(new OrderbookOrder() { Price = ask.Price, Amount = ask.Amount });
                        }

                        var thisOrderbook = Orderbooks.Values.First(x => x.Pair == orderbook.MarketSymbol);
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

        public async Task<ExchangeOrderResult> CreateOrder(string pair, OrderSide side, OrderType type, decimal price, decimal amount)
        {
            var resp = await _client.PlaceOrderAsync(new ExchangeOrderRequest() {
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
