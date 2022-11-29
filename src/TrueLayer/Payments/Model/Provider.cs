using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Provider types
    /// </summary>
    public static class Provider
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

            /// <summary>
            /// Gets the provider Id the PSU will selected for this payment
            /// The field is populated only when a <see cref="GetPaymentResponse"/> is returned
            /// </summary>
            public string? ProviderId { get; init; }

            /// <summary>
            /// Gets the id of the scheme associated to the selected provider that was used to make the payment over.
            /// The field is populated only when a <see cref="GetPaymentResponse"/> is returned
            /// </summary>
            public string? SchemeId { get; init; }
        }

        /// <summary>
        /// Represents provider options that indicates that the provider for this payment is preselected
        /// </summary>
        [JsonDiscriminator("preselected")]
        public record Preselected : IDiscriminated
        {
            public Preselected(string providerId, string schemeId)
            {
                ProviderId = providerId.NotNull(nameof(providerId));
                SchemeId = schemeId.NotNull(nameof(schemeId));
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
            /// Gets the id of the scheme to make the payment over
            /// </summary>
            public string SchemeId { get; }

            /// <summary>
            /// Gets or inits the account details for the remitter
            /// </summary>
            public RemitterAccount? Remitter { get; init; }
        }
    }
}
