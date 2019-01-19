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

namespace Market.API.Exchanges
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
            var socket = _client.GetTickersWebSocket((tickers) =>
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
                    var @event = new PriceUpdatedIntegrationEvent(this.Name, tickers.ElementAt(i).Key, foundMarket.BaseCurrency, foundMarket.AltCurrency, foundMarket.Bid, foundMarket.Ask);
                    _eventBus.Publish(@event);
                }
            });

            socket.Disconnected += SocketStoppedEvent;

            _priceListenerStoppedEvent.WaitOne(); //This thread will block here until the reset event is sent.
            _priceListenerStoppedEvent.Reset();
        }

        private Task SocketStoppedEvent(object sender)
        {
            //Log once logger's implemented
            _priceListenerStoppedEvent.Set();

            return Task.CompletedTask;
        }
    }
}
