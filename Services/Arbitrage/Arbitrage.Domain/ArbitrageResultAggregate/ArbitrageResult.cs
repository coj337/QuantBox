using System.Collections.Generic;

namespace Arbitrage.Domain
{
    public class ArbitrageResult
    {
        public List<ArbitrageStep> Steps { get; set; }

        public decimal Spread { get; set; } // % spread between the initial and final bid/ask

        public long TimePerLoop { get; set; } // Approximately how long arbitrage should take (in seconds)
    }
}
