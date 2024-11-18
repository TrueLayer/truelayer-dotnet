using System;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.SchemeSelection;

namespace TrueLayer.Payments.Model
{
    using PreselectedProviderSchemeSelectionUnion = OneOf<InstantOnly, InstantPreferred, Preselected, UserSelected>;
    using UserSelectedProviderSchemeSelectionUnion = OneOf<InstantOnly, InstantPreferred, UserSelected>;

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
            [Obsolete("The field will be removed soon. Please start using the new <see cref=\"SchemeSelection\"/> field.", error: false)]
            public string? SchemeId { get; init; }

            /// <summary>
            /// Gets or inits the scheme selection preferred to make the payment.
            /// </summary>
            public UserSelectedProviderSchemeSelectionUnion? SchemeSelection { get; init; }
        }

        /// <summary>
        /// Represents provider options that indicates that the provider for this payment is preselected
        /// </summary>
        [JsonDiscriminator("preselected")]
        public record Preselected : IDiscriminated
        {
            public Preselected(string providerId, string? schemeId = null, PreselectedProviderSchemeSelectionUnion? schemeSelection = null)
            {
                if (string.IsNullOrWhiteSpace(schemeId) && schemeSelection is null)
                {
                    throw new ArgumentException("Please specify either the SchemeId or the SchemeSelection option");
                }

                ProviderId = providerId.NotNull(nameof(providerId));
                SchemeId = schemeId;
                SchemeSelection = schemeSelection;
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
            [Obsolete(
                "The field will be removed soon. Please start using the new <see cref=\"SchemeSelection\"/> field.",
                error: false)]
            public string? SchemeId { get; } = null;

            /// <summary>
            /// Gets or inits the account details for the remitter
            /// </summary>
            public RemitterAccount? Remitter { get; init; }

            /// <summary>
            /// Gets or inits the scheme selection preferred to make the payment.
            /// </summary>
            public PreselectedProviderSchemeSelectionUnion? SchemeSelection { get; init; }
        }
    }
}
