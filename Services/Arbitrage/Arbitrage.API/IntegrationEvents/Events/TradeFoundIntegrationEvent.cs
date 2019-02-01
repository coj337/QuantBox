using Arbitrage.Domain;
using BuildingBlocks.EventBus.Events;

namespace Arbitrage.API.IntegrationEvents.Events
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
