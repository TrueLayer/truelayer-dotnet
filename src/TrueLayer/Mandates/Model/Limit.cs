using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public enum PeriodAlignment { Consent = 0, Calendar = 1 }

    public record Limit(int maximumAmount, PeriodAlignment periodAlignment, string value);
}
