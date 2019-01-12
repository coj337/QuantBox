using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain
{
    public class ArbitrageStep
    {
        public Exchange Exchange { get; set; }

        public Pair Pair { get; set; }

        public decimal TransactionFee { get; set; } // Transaction fee incurred to get the asset from first exchange to second

        public decimal NetworkFee { get; set; } // Network fee incurred to get assets out of the exchange (in the alt currency)
    }
}
