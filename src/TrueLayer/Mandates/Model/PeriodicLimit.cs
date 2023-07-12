using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public record PeriodicLimit(
        PeriodicLimitDetail day,
        PeriodicLimitDetail week,
        PeriodicLimitDetail fortnight,
        PeriodicLimitDetail month,
        PeriodicLimitDetail halfYear,
        PeriodicLimitDetail year
    );
}
