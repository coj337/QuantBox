using System;
using System.Collections.Generic;
using System.Text;

namespace Arbitrage.Domain
{
    public class Pair
    {
        public Currency BaseCurrency { get; set; }

        public Currency AltCurrency { get; set; }
    }
}
