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

namespace History.Domain.Exchanges
{
    public class Binance : BackgroundService, IExchange
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        private readonly IConfiguration _configuration;

        private ExchangeBinanceAPI _client;
        protected IReadOnlyCollection<KeyValuePair<string, ExchangeTicker>> Tickers { get; private set; }

        public Binance(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new ExchangeBinanceAPI();

            _client.PublicApiKey = _configuration["Binance:PublicKey"].ToSecureString();
            _client.PrivateApiKey = _configuration["Binance:PrivateKey"].ToSecureString();

            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it, 
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Subscribe to ticker websockets
            using (var socket = _client.GetTickersWebSocket((tickers) =>
            {
                this.Tickers = tickers;
            }))
            {
                Console.ReadLine(); //TODO: Keep it open more gracefully
            }

            throw new NotImplementedException();
        }
    }
}
