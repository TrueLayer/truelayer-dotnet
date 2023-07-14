using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jose;
using OneOf.Types;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// The limits for the payments that can be created by the mandate within a specified time period. At least one periodic limit must be provided upon mandate creation.
    /// </summary>
    /// <param name="Day">Limit for payments made within a day.</param>
    /// <param name="Week">Limit for payments made within a week.</param>
    /// <param name="Fortnight">Limit for payments made within a fortnight.</param>
    /// <param name="Month">Limit for payments made within a month.</param>
    /// <param name="HalfYear">Limit for payments made within 6 months.</param>
    /// <param name="Year">Limit for payments made within a year.</param>
    public record PeriodicLimits(
        Limit? Day,
        Limit? Week,
        Limit? Fortnight,
        Limit? Month,
        Limit? HalfYear,
        Limit? Year
    );
}
