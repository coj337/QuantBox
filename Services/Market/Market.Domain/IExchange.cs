using ExchangeSharp;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Market.Domain
{
    public interface IExchange
    {
        string Name { get; }
        List<Market> Markets { get; }

        bool Authenticate(string publicKey, string privateKey);

        bool IsAuthenticated();

        Task StartPriceListener();
    }
}