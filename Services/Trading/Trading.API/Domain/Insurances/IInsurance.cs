using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.API.Domain.Insurances
{
    //An insurance is a check to be made before a trade is placed (e.g. Overcome fee costs, which will make sure a trade never executes for a loss including fees)
    public interface IInsurance
    {
        bool IsValid();
    }
}
