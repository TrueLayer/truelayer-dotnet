using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// Sets the limits for the payments that can be created by the mandate.
    /// If a payment is attempted that doesn't fit within these constraints it will fail.
    /// </summary>
    /// <param name="MaximumIndividualAmount">A 'cent' value representing the maximum amount that can be specified in a payment instruction.</param>
    /// <param name="PeriodicLimits">The limits for the payments that can be created by the mandate within a specified time period. At least one periodic limit must be provided upon mandate creation.</param>
    /// <param name="ValidFrom">Start date time for which the consent remains valid using format YYYY-MM-DDTHH:mm:ss.sssZ</param>
    /// <param name="ValidTo">End date time for which the consent remains valid using format YYYY-MM-DDTHH:mm:ss.sssZ</param>
    public record Constraints(
        int MaximumIndividualAmount,
        PeriodicLimits PeriodicLimits,
        string? ValidFrom = null,
        string? ValidTo = null);
}
