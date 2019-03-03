using ExchangeManager.Models;
using System.Threading.Tasks;
using Trading.API.Data;

namespace Trading.API.Domain.Actions
{
    public interface IAction
    {
        string Exchange { get; set; }
        Task<TradeResult> Execute(TradingContext context, IExchange exchange);
    }
}
