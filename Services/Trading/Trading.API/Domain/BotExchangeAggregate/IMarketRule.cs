using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.BotExchangeAggregate
{
    public interface IMarketRule
    {
        string Name { get; }
    }
}
