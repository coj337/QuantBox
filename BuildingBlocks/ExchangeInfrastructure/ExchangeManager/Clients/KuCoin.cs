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
        public bool TradingEnabled { get; private set; }
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
            TradingEnabled = true;
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

        public async Task<ExchangeOrderResult> SimulateOrder(string pair, OrderSide side, OrderType type, decimal price, decimal amount, double delaySeconds = 0)
        {
            await Task.Delay((int)(delaySeconds * 1000)); //Wait to simulate real order lag

            ExchangeOrderResult result;
            string feeCurrency;

            if (type == OrderType.Limit)
            {
                var ticker = await _client.GetTickerAsync(pair);

                if (side == OrderSide.Buy)
                {
                    var bestAsk = ticker.Ask;

                }
                else
                {
                    var bestBid = ticker.Bid;

                }

                //TODO: Figure out how to simulate limit orders (Just take best price? Wait for theoretical fill?)
                throw new NotSupportedException();
            }
            else if (type == OrderType.Market)
            {
                var orderbook = await _client.GetOrderBookAsync(pair); //1 request, just like a real order TODO: Pull this from memory once supported (arb services->bots and prices->trading service)

                if (side == OrderSide.Buy)
                {
                    price = PriceCalculator.GetPriceQuote(orderbook.Asks.Values.ToList(), PriceCalculator.ConvertBaseToAlt(orderbook.Asks.First().Value.Price, amount));
                    amount /= price;
                    feeCurrency = pair.Split('-')[1];
                }
                else
                {
                    price = PriceCalculator.GetPriceQuote(orderbook.Bids.Values.ToList(), amount);
                    amount *= price;
                    feeCurrency = pair.Split('-')[0];
                }
                result = new ExchangeOrderResult()
                {
                    MarketSymbol = pair,
                    Price = price,
                    IsBuy = side == OrderSide.Buy,
                    Amount = amount,
                    AmountFilled = amount,
                    AveragePrice = price,
                    Fees = amount * (this.Fee / 100),
                    FeesCurrency = feeCurrency,
                    FillDate = DateTime.Now,
                    OrderDate = DateTime.Now,
                    Result = ExchangeAPIOrderResult.Filled,
                };
            }
            else
            {
                throw new NotSupportedException();
            }

            return result;
        }
    }
}
