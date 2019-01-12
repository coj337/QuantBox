using Arbitrage.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain
{
    public interface IArbitrageHub
    {
        List<ArbitrageResult> ReceiveArbitrageOppurtunities(ArbitrageResult[] arbResults);
    }
}
