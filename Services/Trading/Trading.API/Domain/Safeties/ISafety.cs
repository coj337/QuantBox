using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.Safeties
{
    //A safety is an action that happens independantly of indicators and insurances. (e.g. Deactivate bot on loss, stop-loss, etc.)
    public interface ISafety
    {
        bool IsValid();
        Task TriggerAction();
    }
}
