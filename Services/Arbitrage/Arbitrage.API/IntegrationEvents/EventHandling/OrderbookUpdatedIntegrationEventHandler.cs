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
    public class OrderbookUpdatedIntegrationEventHandler : IIntegrationEventHandler<OrderbookUpdatedIntegrationEvent>
    {
        private readonly ArbitrageService _arbitrageService;

        public OrderbookUpdatedIntegrationEventHandler(ArbitrageService arbitrageService)
        {
            _arbitrageService = arbitrageService;
        }

        public Task Handle(OrderbookUpdatedIntegrationEvent @event)
        {
            _arbitrageService.UpdatePrice(new ExchangeData(@event.Exchange, @event.Pair, @event.BaseCurrency, @event.AltCurrency, @event.NewBids, @event.NewAsks));

            return Task.CompletedTask;
        }
    }
}
