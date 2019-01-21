using ExchangeSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Market.Domain;
using BuildingBlocks.EventBus.Abstractions;
using Market.API.IntegrationEvents.Events;

namespace Market.API.Services.Exchanges
{
    public class Binance : IExchange
    {
        private readonly IEventBus _eventBus;
        private readonly ExchangeBinanceAPI _client;

        public string Name { get => "Binance"; }
        public Dictionary<string, MarketData> Markets { get; private set; }
        public List<CurrencyData> Currencies { get; private set; }

        public Binance(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _client = new ExchangeBinanceAPI();
            Markets = new Dictionary<string, MarketData>();
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

        private readonly ManualResetEvent _priceListenerStoppedEvent = new ManualResetEvent(false);

        public async Task StartPriceListener()
        {
            var currencies = await _client.GetCurrenciesAsync();
            foreach(var currency in currencies)
            {
                Currencies.Add(new CurrencyData() {
                    Name = currency.Value.FullName,
                    Symbol = currency.Key,
                    DepositEnabled = currency.Value.DepositEnabled,
                    WithdrawalEnabled = currency.Value.WithdrawalEnabled,
                    WithdrawalFee = currency.Value.TxFee,
                    MinWithdrawal = currency.Value.MinWithdrawalSize,
                    MinConfirmations = currency.Value.MinConfirmations
                });

                //Send integration event for services that just need assets
                var @newAssetEvent = new AssetAddedIntegrationEvent(currency.Key, currency.Value.FullName);
                _eventBus.Publish(@newAssetEvent);
            }

            //Subscribe to ticker websockets
            /*var socket = _client.GetTickersWebSocket((tickers) =>
            {
                for (var i = 0; i < tickers.Count(); i++)
                {
                    MarketData foundMarket;
                    var ticker = tickers.ElementAt(i);

                    if (!Markets.ContainsKey(ticker.Key))
                    {
                        //A new currency has appeared in the sockets (or this is first start), add it
                        Markets.Add(ticker.Key, new MarketData());
                        //TODO: Find name and send integration event on new symbol
                    }
                    foundMarket = Markets[ticker.Key];

                    //Update the market
                    foundMarket.Pair = ticker.Key;
                    foundMarket.AltCurrency = ticker.Key.Substring(0, 3);
                    foundMarket.BaseCurrency = ticker.Key.Substring(3);
                    foundMarket.Bid = ticker.Value.Bid;
                    foundMarket.Ask = ticker.Value.Ask;
                    foundMarket.BaseVolume = ticker.Value.Volume.BaseCurrencyVolume;
                    foundMarket.AltVolume = ticker.Value.Volume.QuoteCurrencyVolume;

                    //Send integration event for other services
                    //Disabled since nothings using them right now
                    //var @event = new PriceUpdatedIntegrationEvent(this.Name, tickers.ElementAt(i).Key, foundMarket.BaseCurrency, foundMarket.AltCurrency, foundMarket.Bid, foundMarket.Ask);
                    //_eventBus.Publish(@event);
                }
            });

            socket.Disconnected += SocketStoppedEvent;

            _priceListenerStoppedEvent.WaitOne(); //This thread will block here until the reset event is sent.
            _priceListenerStoppedEvent.Reset();*/
        }

        private Task SocketStoppedEvent(object sender)
        {
            //Log once logger's implemented
            _priceListenerStoppedEvent.Set();

            return Task.CompletedTask;
        }

        private readonly ManualResetEvent _orderbookListenerStoppedEvent = new ManualResetEvent(false);

        public async Task StartOrderbookListener()
        {
            /////TEMP WHILE WE AREN'T USING THE OTHER SOCKET
            var markets = await _client.GetMarketSymbolsMetadataAsync();
            foreach(var market in markets)
            {
                Markets.Add(market.MarketSymbol, new MarketData()
                {
                    Pair = market.MarketSymbol,
                    BaseCurrency = market.QuoteCurrency,
                    AltCurrency = market.BaseCurrency
                });
            }

            //Subscribe to ticker websockets
            var socket = _client.GetFullOrderBookWebSocket((orderbook) =>
            {
                List<Order> bids = new List<Order>();
                List<Order> asks = new List<Order>();

                foreach(var bid in orderbook.Bids.Values)
                {
                    bids.Add(new Order() {Price = bid.Price, Amount = bid.Amount });
                }
                foreach (var ask in orderbook.Asks.Values)
                {
                    asks.Add(new Order() { Price = ask.Price, Amount = ask.Amount });
                }
                
                //Send integration event for other services
                if (Markets.ContainsKey(orderbook.MarketSymbol)) {
                    var @event = new OrderbookUpdatedIntegrationEvent(this.Name, orderbook.MarketSymbol, Markets[orderbook.MarketSymbol].BaseCurrency, Markets[orderbook.MarketSymbol].AltCurrency, bids, asks);
                    _eventBus.Publish(@event);
                }
            });

            socket.Disconnected += OrderbookSocketStoppedEvent;

            _orderbookListenerStoppedEvent.WaitOne(); //This thread will block here until the reset event is sent.
            _orderbookListenerStoppedEvent.Reset();
        }

        private Task OrderbookSocketStoppedEvent(object sender)
        {
            //Log once logger's implemented
            _orderbookListenerStoppedEvent.Set();

            return Task.CompletedTask;
        }
    }
}
