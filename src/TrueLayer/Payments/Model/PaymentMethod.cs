using OneOf;
using static TrueLayer.Payments.Model.Provider;
using static TrueLayer.Payments.Model.Beneficiary;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    using ProviderUnion = OneOf<UserSelected>;
    using BeneficiaryUnion = OneOf<MerchantAccount, ExternalAccount>;

    /// <summary>
    /// Payment Method types
    /// </summary>
    public static class PaymentMethod
    {
        /// <summary>
        /// Defines a payment via Bank Transfer
        /// </summary>
        [JsonDiscriminator("bank_transfer")]
        public record BankTransfer : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="BankTransfer"/>
            /// </summary>
            /// <param name="providerSelection">The options for selecting a provider for the payment</param>
            /// <param name="beneficiary">The details of the payment destination</param>
            public BankTransfer(ProviderUnion providerSelection, BeneficiaryUnion beneficiary)
            {
                ProviderSelection = providerSelection.NotNull(nameof(providerSelection));
                Beneficiary = beneficiary.NotNull(nameof(beneficiary));
            }

            /// <summary>
            /// Gets the payment method type
            /// </summary>
            public string Type => "bank_transfer";

            /// <summary>
            /// Gets or sets the provider selection options
            /// </summary>
            public ProviderUnion ProviderSelection { get; init; }

            /// <summary>
            /// Gets or sets the beneficiary details
            /// </summary>
            public BeneficiaryUnion Beneficiary { get; init; }
        }
    }
}
