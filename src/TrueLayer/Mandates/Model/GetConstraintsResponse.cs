using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jose;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// Gets the limits for the payments that can be created by the mandate.
    /// If a payment is attempted that doesn't fit within these constraints it will fail.
    /// </summary>
    /// <param name="ValidFrom">Start date time for which the consent remains valid.</param>
    /// <param name="ValidTo">End date time for which the consent remains valid.</param>
    /// <param name="MaximumIndividualAmount">A 'cent' value representing the maximum amount that can be specified in a payment instruction.</param>
    /// <param name="PeriodicLimits">The state of the constraints utilisation within each periodic limit defined in the mandate creation. There will always be at least 1 period state defined.</param>
    public record GetConstraintsResponse(
        DateTime? ValidFrom,
        DateTime? ValidTo,
        int MaximumIndividualAmount,
        PeriodicLimit PeriodicLimits);
}

