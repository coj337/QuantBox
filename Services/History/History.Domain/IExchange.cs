using ExchangeSharp;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace History.Domain
{
    public interface IExchange : IHostedService
    {
        //Dictionary<string, ExchangeTicker> Tickers { get; }
    }
}
