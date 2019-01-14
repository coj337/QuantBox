using BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arbitrage.API.IntegrationEvents.Events
{
    public class PriceUpdatedIntegrationEvent : IntegrationEvent
    {
        public string Exchange { get; set; }
        public string Pair { get; set; }
        public decimal NewBid { get; set; }
        public decimal NewAsk { get; set; }

        public PriceUpdatedIntegrationEvent(string exchange, string pair, decimal newBid, decimal newAsk)
        {
            Pair = pair;
            NewBid = newBid;
            NewAsk = newAsk;
            Exchange = exchange;
        }
    }
}
