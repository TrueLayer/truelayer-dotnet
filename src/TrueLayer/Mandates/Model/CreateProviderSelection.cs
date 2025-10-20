using TrueLayer.Payments.Model;
using TrueLayer.Serialization;

namespace TrueLayer.Mandates.Model;

/// <summary>
/// Provider selection types for CREATE payment requests
/// </summary>
public static class CreateProviderSelection
{
    /// <summary>
    /// Represents provider options that indicates that the provider is to be selected from a collection
    /// </summary>
    [JsonDiscriminator("user_selected")]
    public record UserSelected : IDiscriminated
    {
        /// <summary>
        /// Gets the provider type
        /// </summary>
        public string Type => "user_selected";

        /// <summary>
        /// Gets or inits the filter used to determine the banks that should be displayed on the bank selection screen
        /// </summary>
        public ProviderFilter? Filter { get; init; }
    }

    /// <summary>
    /// Represents provider options that indicates that the provider for this payment is preselected
    /// </summary>
    [JsonDiscriminator("preselected")]
    public record Preselected : IDiscriminated
    {
        public Preselected(string providerId)
        {
            ProviderId = providerId.NotNull(nameof(providerId));
        }

        /// <summary>
        /// Gets the provider type
        /// </summary>
        public string Type => "preselected";

        /// <summary>
        /// Gets the provider Id the PSU will use for this payment
        /// </summary>
        public string ProviderId { get; }

        /// <summary>
        /// Gets or inits the account details for the remitter
        /// </summary>
        public RemitterAccount? Remitter { get; init; }
    }
}
