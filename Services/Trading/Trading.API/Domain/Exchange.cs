using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain
{
    public class Exchange
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool TradingEnabled { get; set; }
    }
}
