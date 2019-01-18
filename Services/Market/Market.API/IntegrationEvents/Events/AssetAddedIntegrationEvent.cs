using BuildingBlocks.EventBus.Events;

namespace Market.API.IntegrationEvents.Events
{
    public class AssetAddedIntegrationEvent : IntegrationEvent
    {
        public string Symbol { get; set; }
        public string Name { get; set; }

        public AssetAddedIntegrationEvent(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }
}
