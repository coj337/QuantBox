using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.ViewModels
{
    public class BotPreviewViewModel
    {
        public string Name { get; set; }
        public decimal Profit { get; set; }
        public bool TradingEnabled { get; set; }
    }
}
