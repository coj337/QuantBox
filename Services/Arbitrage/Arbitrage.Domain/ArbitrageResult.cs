using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain
{
    public class ArbitrageResult
    {
        public string Pair { get; set; }

        public SupportedExchange startExchange { get; set; }

        public SupportedExchange endExchange { get; set; }

        public decimal spread { get; set; }
    }
}
