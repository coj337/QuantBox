using Arbitrage.Domain;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Arbitrage.Infrastructure
{
    public class ArbitrageHub : Hub<IArbitrageHub>
    {

    }
}
