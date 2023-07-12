using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public record Constraints(
        string validFrom,
        string validTo,
        int maximumIndividualAmount,
        PeriodicLimits periodicLimits);
}
