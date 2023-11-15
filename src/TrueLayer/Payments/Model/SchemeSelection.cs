using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model;

/// <summary>
/// Scheme selection types
/// </summary>
public static class SchemeSelection
{
    /// <summary>
    /// Represents that the scheme for the payment allows only providers that support instant payments.
    /// </summary>
    [JsonDiscriminator("instant_only")]
    public record InstantOnly : IDiscriminated
    {
        /// <summary>
        /// Gets the scheme selection type
        /// </summary>
        public string Type => "instant_only";

        /// <summary>
        /// Gets or inits the setting to allow providers to possibly charge the remitter with a transaction fee.
        /// If false, only providers supporting schemes that are free will be available to select in the provider
        /// selection action.
        /// Unless explicitly set, will default to false.
        /// </summary>
        public bool AllowRemitterFee { get; init; } = false;
    }

    /// <summary>
    /// Represents that the scheme for the payment will prefer providers that allow instant payments,
    /// but allow defaulting back to non-instant payments if unavailable.
    /// </summary>
    [JsonDiscriminator("instant_preferred")]
    public record InstantPreferred : IDiscriminated
    {
        /// <summary>
        /// Gets the scheme selection type
        /// </summary>
        public string Type => "instant_preferred";

        /// <summary>
        /// Gets or inits the setting to allow providers to possibly charge the remitter with a transaction fee.
        /// If false, only providers supporting schemes that are free will be available to select in the provider selection action.
        /// Unless explicitly set, will default to false.
        /// </summary>
        public bool AllowRemitterFee { get; init; } = false;
    }

    /// <summary>
    /// Represents that the scheme for the payment is selected from a collection.
    /// </summary>
    [JsonDiscriminator("user_selected")]
    public record UserSelected : IDiscriminated
    {
        /// <summary>
        /// Gets the scheme selection type
        /// </summary>
        public string Type => "user_selected";
    }

    /// <summary>
    /// Represents that the scheme for the payment is preselected.
    /// </summary>
    [JsonDiscriminator("preselected")]
    public record Preselected : IDiscriminated
    {
        /// <summary>
        /// Gets the scheme selection type
        /// </summary>
        public string Type => "preselected";
    }
}
