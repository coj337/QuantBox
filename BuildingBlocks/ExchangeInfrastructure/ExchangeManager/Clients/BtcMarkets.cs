﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BtcMarketsApiClient;
using System.Threading;
using ExchangeManager.Models;
using ExchangeSharp;
using OrderType = ExchangeManager.Models.OrderType;
using ExchangeManager.Helpers;

namespace ExchangeManager.Clients
{
    public class BtcMarkets : IExchange
    {
        private readonly BtcMarketsClient _client;        
        public string Name => "BtcMarkets";
        public decimal Fee => 0.22m;
        public bool TradingEnabled => true;
        public bool IsAuthenticated { get; private set; }
        public Dictionary<string, Orderbook> Orderbooks { get; private set; }
        public Dictionary<string, CurrencyData> Currencies { get; private set; }

        public BtcMarkets()
        {
            _client = new BtcMarketsClient();
            Orderbooks = new Dictionary<string, Orderbook>();
            Currencies = new Dictionary<string, CurrencyData>();
        }

        public bool Authenticate(string publicKey, string privateKey)
        {
            _client.SetCredentials(publicKey, privateKey);

            //Make sure auth didn't fail
            return !_client.RetrieveAccountBalance().Contains("Authentication failed");
        }

        public Task StartOrderbookListener()
        {
            var markets = _client.GetMarkets();

            foreach(var market in markets)
            {
                var orderbook = _client.GetOrderBook(market.Pair);
                Orderbooks.Add(market.Instrument + "/" + market.Currency, 
                    new Orderbook()
                    {
                        Pair = market.Pair,
                        BaseCurrency = market.Currency,
                        AltCurrency = market.Instrument,
                        Asks = orderbook.asks.Select(ask => new OrderbookOrder() { Price = ask[0], Amount = ask[1] }).ToList(),
                        Bids = orderbook.bids.Select(bid => new OrderbookOrder() { Price = bid[0], Amount = bid[1] }).ToList()
                    }
                );
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

                            List<OrderbookOrder> bids = new List<OrderbookOrder>();
                            List<OrderbookOrder> asks = new List<OrderbookOrder>();
                            bids.AddRange(orderbook.bids.Select(bid => new OrderbookOrder() { Price = bid[0], Amount = bid[1] }));
                            asks.AddRange(orderbook.asks.Select(ask => new OrderbookOrder() { Price = ask[0], Amount = ask[1] }));

                            var thisOrderbook = Orderbooks[market.Instrument + "/" + market.Currency];
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

        public async Task<ExchangeOrderResult> CreateOrder(string pair, OrderSide side, OrderType type, decimal price, decimal amount)
        {
            string[] currencies = pair.Split('/');
            string orderSide = side == OrderSide.Buy ? "Ask" : "Bid";
            string orderType = type == OrderType.Market ? "Market" : "Limit";

            string orderId = _client.CreateNewOrder(currencies[0], currencies[1], Convert.ToInt64(price * 100000000), Convert.ToInt32(amount * 100000000), orderSide, orderType);

            return new ExchangeOrderResult()
            {
                OrderId = orderId
            };
        }

        public async Task<ExchangeOrderResult> SimulateOrder(string pair, OrderSide side, OrderType type, decimal price, decimal amount, double delaySeconds = 0)
        {
            await Task.Delay((int)(delaySeconds * 1000)); //Wait to simulate real order lag

            ExchangeOrderResult result = new ExchangeOrderResult();

            if (type == OrderType.Limit)
            {
                var ticker = _client.GetTicker(pair);

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
                var orderbook = _client.GetOrderBook(pair); //1 request, just like a real order TODO: Pull this from memory once supported (arb services->bots and prices->trading service)

                if (side == OrderSide.Buy)
                {
                    price = PriceCalculator.GetPriceQuote(orderbook.asks.Select(ask => new OrderbookOrder() { Price = ask[0], Amount = ask[1] }).ToList(), PriceCalculator.ConvertBaseToAlt(orderbook.asks.First()[0], amount));
                    amount /= price;
                }
                else
                {
                    price = PriceCalculator.GetPriceQuote(orderbook.bids.Select(bid => new OrderbookOrder() { Price = bid[0], Amount = bid[1] }).ToList(), amount);
                    amount *= price;
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
                    FeesCurrency = "Alt",
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
