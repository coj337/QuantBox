using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeManager.Models
{
    public interface IExchange
    {
        string Name { get; }

        decimal Fee { get; } //The % trade fee on the exchange (e.g. 0.1 for 0.1%) 

        List<Orderbook> Orderbooks { get; }

        List<CurrencyData> Currencies { get; }

        bool Authenticate(string publicKey, string privateKey);

        bool IsAuthenticated();

        Task StartOrderbookListener();
    }
}