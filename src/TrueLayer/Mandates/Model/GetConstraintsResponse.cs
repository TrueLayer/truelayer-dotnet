using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jose;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// Retrieve the constriants defined on the mandate, as well as the current utilisation of those constraints within the periods.
    /// </summary>
    /// <param name="MaximumIndividualAmount">A 'cent' value representing the maximum amount that can be specified in a payment instruction.</param>
    /// <param name="PeriodicLimits">The state of the constraints utilisation within each periodic limit defined in the mandate creation. There will always be at least 1 period state defined.</param>
    /// <param name="ValidFrom">Start date time for which the consent remains valid.</param>
    /// <param name="ValidTo">End date time for which the consent remains valid.</param>
    internal record GetConstraintsResponse(
        int MaximumIndividualAmount,
        PeriodicLimit PeriodicLimits,
        DateTime? ValidFrom = null,
        DateTime? ValidTo = null);
}

