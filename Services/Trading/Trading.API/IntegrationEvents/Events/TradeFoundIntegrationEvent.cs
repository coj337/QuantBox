using BuildingBlocks.EventBus.Events;
using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Domain;

namespace Trading.API.IntegrationEvents.Events
{
    public class ArbitrageFoundIntegrationEvent : IntegrationEvent
    {
        public ArbitrageResult Result { get; set; }

        public ArbitrageFoundIntegrationEvent(ArbitrageResult result)
        {
            Result = result;
        }
    }
}
