using Arbitrage.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Domain
{
    public interface IArbitrageHub
    {
        Task ReceiveTriangleArbitrage(ArbitrageResult arbResult);
    }
}
