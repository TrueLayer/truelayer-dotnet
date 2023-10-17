using OneOf;
using static TrueLayer.Payments.Model.Provider;
using static TrueLayer.Payments.Model.Beneficiary;
using static TrueLayer.Payments.Model.Retry;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    using ProviderUnion = OneOf<UserSelected, Preselected>;
    using BeneficiaryUnion = OneOf<MerchantAccount, ExternalAccount>;
    using RetryUnion = OneOf<Standard, Smart>;

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
            /// Gets or inits the provider selection options
            /// </summary>
            public ProviderUnion ProviderSelection { get; init; }

            /// <summary>
            /// Gets or inits the beneficiary details
            /// </summary>
            public BeneficiaryUnion Beneficiary { get; init; }
        }

        /// <summary>
        /// Defines a payment via Mandate
        /// </summary>
        [JsonDiscriminator("mandate")]
        public record Mandate : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="Mandate"/>
            /// </summary>
            /// <param name="mandateId">The identifier of the mandate</param>
            /// <param name="reference">The payment reference, useful for reconciliation needs</param>
            public Mandate(string mandateId, string? reference, RetryUnion? retry)
            {
                MandateId = mandateId.NotNullOrWhiteSpace(nameof(mandateId));
                Reference = reference.NotEmptyOrWhiteSpace(nameof(reference));
                Retry = retry;
            }

            /// <summary>
            /// Gets the payment method type
            /// </summary>
            public string Type => "mandate";

            /// <summary>
            /// The identifier of the mandate
            /// </summary>
            public string MandateId { get; init; }

            /// <summary>
            /// The payment reference, useful for reconciliation needs
            /// </summary>
            public string? Reference { get; init; }

            public RetryUnion? Retry { get; init; }
        }
    }
}
