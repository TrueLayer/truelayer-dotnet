namespace TrueLayer.Mandates.Model
{
    public enum PeriodAlignment { Consent = 0, Calendar = 1 }

    /// <summary>
    /// The state of the constraints utilisation within the periodic limit defined in the mandate creation
    /// </summary>
    /// <param name="MaximumAmount">A 'cent' value representing the maximum cumulative amount that all successful payments can claim in the period. This might be less than the maximum_amount requested due to proration on calendar aligned periods.</param>
    /// <param name="PeriodAlignment">Specifies whether the period starts on the date of consent creation or lines up with a calendar. If the PeriodAlignment is calendar, the limit is pro-rated in the first period to the remaining number of days.</param>
    public record Limit(int MaximumAmount, PeriodAlignment PeriodAlignment);
}
