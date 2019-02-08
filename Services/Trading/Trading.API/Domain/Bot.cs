using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain
{
    public class Bot
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ExchangeConfig> Accounts { get; set; }
        public bool TradingEnabled { get; set; }

        public Bot()
        {
            Accounts = new List<ExchangeConfig>();
        }
    }
}
