using Arbitrage.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Infrastructure
{
    public interface IArbitrageHub
    {
        List<ArbitrageResult> ReceiveArbitrageMatrix();
    }
}
