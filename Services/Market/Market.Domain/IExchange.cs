using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Market.Domain
{
    public interface IExchange : IHostedService
    {
        string Name { get; }

        Dictionary<string, MarketData> Markets { get; }

        bool Authenticate(string publicKey, string privateKey);

        bool IsAuthenticated();

        Task StartPriceListener();
    }
}