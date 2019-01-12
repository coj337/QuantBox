using System.Collections.Generic;

namespace Arbitrage.Domain
{
    public class Exchange
    {
        public string Name { get; set; }

        public decimal TradeFee { get; set; } //The fee % (e.g. 0.2 for 0.2%)

        public List<Pair> Pairs { get; set; }
    }
}
