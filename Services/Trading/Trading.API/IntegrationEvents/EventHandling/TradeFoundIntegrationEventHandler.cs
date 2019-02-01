using BuildingBlocks.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.IntegrationEvents.Events;
using Trading.API.Services;

namespace Trading.API.IntegrationEvents.EventHandling
{
    public class ArbitrageFoundIntegrationEventHandler : IIntegrationEventHandler<ArbitrageFoundIntegrationEvent>
    {
        private readonly TradingService _tradingService;

        public ArbitrageFoundIntegrationEventHandler(TradingService tradingService)
        {
            _tradingService = tradingService;
        }

        public async Task Handle(ArbitrageFoundIntegrationEvent @event)
        {
            await _tradingService.ExecuteArbitrage(@event.Result);
        }
    }
}
