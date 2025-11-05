using System;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.SchemeSelection;

namespace TrueLayer.Payments.Model;

using PreselectedProviderSchemeSelectionUnion = OneOf<InstantOnly, InstantPreferred, Preselected, UserSelected>;
using UserSelectedProviderSchemeSelectionUnion = OneOf<InstantOnly, InstantPreferred, UserSelected>;

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

        /// <summary>
        /// Gets or inits the scheme selection preferred to make the payment.
        /// </summary>
        public UserSelectedProviderSchemeSelectionUnion? SchemeSelection { get; init; }

        /// <summary>
        /// Gets or inits the account details for the remitter
        /// </summary>
        public UserSelectedRemitterAccount? Remitter { get; init; }
    }

    /// <summary>
    /// Represents provider options that indicates that the provider for this payment is preselected
    /// </summary>
    [JsonDiscriminator("preselected")]
    public record Preselected : IDiscriminated
    {
        /// <summary>
        /// Creates a new <see cref="Preselected"/> instance with the specified provider and scheme selection.
        /// </summary>
        /// <param name="providerId">The provider Id the PSU will use for this payment.</param>
        /// <param name="schemeSelection">The scheme selection option for the payment.</param>
        public Preselected(string providerId, PreselectedProviderSchemeSelectionUnion? schemeSelection = null)
        {
            if (schemeSelection is null)
            {
                throw new ArgumentException("Please specify the SchemeSelection option", nameof(schemeSelection));
            }

            ProviderId = providerId.NotNull(nameof(providerId));
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
        /// Gets or inits the account details for the remitter
        /// </summary>
        public RemitterAccount? Remitter { get; init; }

        /// <summary>
        /// Gets or inits the scheme selection preferred to make the payment.
        /// </summary>
        public PreselectedProviderSchemeSelectionUnion? SchemeSelection { get; init; }
    }
}
