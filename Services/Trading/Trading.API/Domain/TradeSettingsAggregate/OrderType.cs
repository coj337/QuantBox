using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.TradeSettingsAggregate
{
    public enum OrderType
    {
        Limit, //TODO: Make these both implementations of an interface instead (with timeouts etc)
        Market
    }
}
