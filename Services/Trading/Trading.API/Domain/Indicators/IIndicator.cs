using ExchangeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.Indicators
{
    //An indicator can be any trading indicator (e.g. RSI, MACD, etc.)
    public interface IIndicator
    {
        bool IsValid(List<IExchange> exchanges);
    }
}
