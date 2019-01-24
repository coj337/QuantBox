using System.Collections.Generic;

namespace Arbitrage.Domain
{
    public class ArbitrageResult
    {
        public string Exchange { get; set; }

        public string Path { get; set; }

        public decimal TransactionFee { get; set; } // Transaction fee incurred to get the asset from first exchange to second

        public decimal NetworkFee { get; set; } // Network fee incurred to get assets out of the exchange (in the alt currency)

        public decimal Profit { get; set; } // % spread between the initial and final bid/ask

        public long TimePerLoop { get; set; } // Approximately how long arbitrage should take (in seconds)
    }
}
