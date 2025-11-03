using TrueLayer.Serialization;

namespace TrueLayer.Payouts.Model;

/// <summary>
/// Payout scheme selection types
/// </summary>
public static class SchemeSelection
{
    /// <summary>
    /// Automatically select a payment scheme that supports instant payments based on currency and geography.
    /// </summary>
    [JsonDiscriminator("instant_only")]
    public record InstantOnly : IDiscriminated
    {
        /// <summary>
        /// Gets the scheme selection type
        /// </summary>
        public string Type => "instant_only";
    }

    /// <summary>
    /// Automatically select a payment scheme that supports instant payments based on currency and geography,
    /// with a fallback to a non-instant scheme if instant payment is unavailable.
    /// The payout_executed webhook will specify the actual scheme used.
    /// This is optimal when slow settlement is not a concern. This is used by default if no scheme_selection is provided.
    /// </summary>
    [JsonDiscriminator("instant_preferred")]
    public record InstantPreferred : IDiscriminated
    {
        /// <summary>
        /// Gets the scheme selection type
        /// </summary>
        public string Type => "instant_preferred";
    }


    /// <summary>
    /// Represents that the scheme for the payout is preselected.
    /// </summary>
    [JsonDiscriminator("preselected")]
    public record Preselected : IDiscriminated
    {
        /// <summary>
        /// Gets the scheme selection type
        /// </summary>
        public string Type => "preselected";

        /// <summary>
        /// Select a payment scheme compatible with the currency and geographic region to avoid payout failures after submission.
        /// This helps with payouts by selecting the better-performing scheme between two similar options in a region, based on various criteria
        /// </summary>
        public string? SchemeId { get; init; }
    }
}
