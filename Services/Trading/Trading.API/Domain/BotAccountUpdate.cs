using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain
{
    public class BotAccountUpdate
    {
        public string BotId { get; set; }
        public string Exchange { get; set; }
        public string Account { get; set; }
    }
}
