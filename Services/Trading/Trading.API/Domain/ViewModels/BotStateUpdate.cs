using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain
{
    public class BotStateUpdate
    {
        public string BotId { get; set; }
        public bool State { get; set; }
    }
}
