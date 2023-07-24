using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jose;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// The state of the constraints utilisation within each periodic limit defined in the mandate creation. There will always be at least 1 period state defined
    /// </summary>
    /// <param name="Day">Utilisation of this mandate within the current day.</param>
    /// <param name="Week">Utilisation of this mandate within the current week period.</param>
    /// <param name="Fortnight">Utilisation of this mandate within the current fortnight period.</param>
    /// <param name="Month">Utilisation of this mandate within the current month period.</param>
    /// <param name="HalfYear">Utilisation of this mandate within the current half-year period.</param>
    /// <param name="Year">Utilisation of this mandate within the current year period.</param>
    internal record PeriodicLimit(
        PeriodicLimitDetail? Day = null,
        PeriodicLimitDetail? Week = null,
        PeriodicLimitDetail? Fortnight = null,
        PeriodicLimitDetail? Month = null,
        PeriodicLimitDetail? HalfYear = null,
        PeriodicLimitDetail? Year = null
    );
}
