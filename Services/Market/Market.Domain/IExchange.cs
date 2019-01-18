using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Market.Domain
{
    public interface IExchange
    {
        string Name { get; }

        Dictionary<string, MarketData> Markets { get; }

        List<CurrencyData> Currencies { get; }

        bool Authenticate(string publicKey, string privateKey);

        bool IsAuthenticated();

        Task StartPriceListener();
    }
}