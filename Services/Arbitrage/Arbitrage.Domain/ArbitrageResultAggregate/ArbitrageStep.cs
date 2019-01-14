using Arbitrage.Domain.ArbitrageResultAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain
{
    public class ArbitrageStep
    {
        public ExchangeData Exchange { get; set; }

        public PairData Pair { get; set; }

        public decimal TransactionFee { get; set; } // Transaction fee incurred to get the asset from first exchange to second

        public decimal NetworkFee { get; set; } // Network fee incurred to get assets out of the exchange (in the alt currency)
    }
}
