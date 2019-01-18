using BuildingBlocks.EventBus.Abstractions;
using Sentiment.API.IntegrationEvents.Events;
using Sentiment.Domain;
using Sentiment.Infrastructure;
using System.Threading.Tasks;

namespace Sentiment.API.IntegrationEvents.EventHandling
{
    public class AssetAddedIntegrationEventHandler : IIntegrationEventHandler<AssetAddedIntegrationEvent>
    {
        private readonly SentimentService _sentimentService;

        public AssetAddedIntegrationEventHandler(SentimentService sentimentService)
        {
            _sentimentService = sentimentService;
        }

        public Task Handle(AssetAddedIntegrationEvent @event)
        {
            _sentimentService.AddAsset(new SentimentAsset() { symbol = @event.Symbol, name = @event.Name });

            return Task.CompletedTask;
        }
    }
}
