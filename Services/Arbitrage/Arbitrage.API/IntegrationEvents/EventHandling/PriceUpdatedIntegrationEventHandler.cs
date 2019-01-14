using BuildingBlocks.EventBus.Abstractions;
using Arbitrage.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arbitrage.Infrastructure;
using Arbitrage.Domain;
using Arbitrage.Domain.ExchangeAggregate;

namespace Arbitrage.API.IntegrationEvents.EventHandling
{
    public class PriceUpdatedIntegrationEventHandler : IIntegrationEventHandler<PriceUpdatedIntegrationEvent>
    {
        private readonly ArbitrageService _arbitrageService;

        public PriceUpdatedIntegrationEventHandler(ArbitrageService arbitrageService)
        {
            _arbitrageService = arbitrageService;
        }

        public Task Handle(PriceUpdatedIntegrationEvent @event)
        {
            _arbitrageService.UpdatePrice(new ExchangeData(@event.Exchange, @event.Pair, @event.NewBid, @event.NewAsk));

            return Task.CompletedTask;
        }
    }
}
