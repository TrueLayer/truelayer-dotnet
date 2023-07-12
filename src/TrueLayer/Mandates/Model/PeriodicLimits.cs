using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public record PeriodicLimits(
        Limit day,
        Limit week,
        Limit fortnight,
        Limit month,
        Limit halfYear,
        Limit year
    );
}
