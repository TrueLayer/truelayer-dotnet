using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public record PeriodicLimitDetail(string startDate, string endDate, int currentAmount, int maximumAvailableAmount, string periodAlignment);
}
