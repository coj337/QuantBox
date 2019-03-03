using ExchangeManager.Models;
using System.Collections.Generic;
using Trading.API.Domain.BotExchangeAggregate;

namespace Trading.API.Domain
{
    public class BotExchange
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ExchangeConfig SelectedConfig { get; set; }
        public List<BotPair> Markets { get; set; } //List of pairs that are supported (e.g. Pair == "BTC/ETH" or Pair.BaseCurrency == "BTC" and Pair.AltCurrency == "ETH")

        public BotExchange(string name)
        {
            Name = name;
            Markets = new List<BotPair>();
        }
    }
}
