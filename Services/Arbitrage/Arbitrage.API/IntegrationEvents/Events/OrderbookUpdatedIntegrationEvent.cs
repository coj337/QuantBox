using Arbitrage.Domain;
using BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arbitrage.API.IntegrationEvents.Events
{
    public class OrderbookUpdatedIntegrationEvent : IntegrationEvent
    {
        public string Exchange { get; set; }
        public string Pair { get; set; }
        public string BaseCurrency { get; set; }
        public string AltCurrency { get; set; }
        public List<Order> NewBids { get; set; }
        public List<Order> NewAsks { get; set; }

        public OrderbookUpdatedIntegrationEvent(string exchange, string pair, string baseCurrency, string altCurrency, List<Order> newBids, List<Order> newAsks)
        {
            Pair = pair;
            BaseCurrency = baseCurrency;
            AltCurrency = altCurrency;
            NewBids = newBids;
            NewAsks = newAsks;
            Exchange = exchange;
        }
    }
}
