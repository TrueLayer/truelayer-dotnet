using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PeriodAlignment { Consent = 0, Calendar = 1 }

    /// <summary>
    /// The state of the constraints utilisation within the periodic limit defined in the mandate creation
    /// </summary>
    /// <param name="MaximumAvailableAmount">A 'cent' value representing the maximum cumulative amount that all successful payments can claim in the period. This might be less than the maximum_amount requested due to proration on calendar aligned periods.</param>
    /// <param name="CurrentAmount">A 'cent' value representing the current cumulative amount that all successful payments have claimed in the period.</param>
    /// <param name="StartDate">The start date for the current period.</param>
    /// <param name="EndDate">The end date for the current period, and start of the next period.</param>
    /// <param name="PeriodAlignment">Specifies whether the period starts on the date of consent creation or lines up with a calendar. If the PeriodAlignment is calendar, the limit is pro-rated in the first period to the remaining number of days.</param>
    public record Limit(
        int MaximumAvailableAmount,
        int CurrentAmount,
        DateTime StartDate,
        DateTime EndDate,
        PeriodAlignment PeriodAlignment);
}
