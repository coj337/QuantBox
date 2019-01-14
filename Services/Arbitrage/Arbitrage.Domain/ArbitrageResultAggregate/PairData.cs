using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain.ArbitrageResultAggregate
{
    public class PairData
    {
        public string Pair { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
    }
}
