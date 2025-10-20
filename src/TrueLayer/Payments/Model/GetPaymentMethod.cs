using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.Beneficiary;
using static TrueLayer.Payments.Model.GetProvider;
using static TrueLayer.Payments.Model.Retry;

namespace TrueLayer.Payments.Model;

using BeneficiaryUnion = OneOf<MerchantAccount, ExternalAccount>;
using GetProviderUnion = OneOf<UserSelected, Preselected>;
using RetryUnion = OneOf<Standard, Smart>;

/// <summary>
/// Payment Method types for GET payment responses
/// </summary>
public static class GetPaymentMethod
{
    /// <summary>
    /// Defines a payment via Bank Transfer
    /// </summary>
    [JsonDiscriminator("bank_transfer")]
    public record BankTransfer : IDiscriminated
    {
        /// <summary>
        /// Gets the payment method type
        /// </summary>
        public string Type => "bank_transfer";

        /// <summary>
        /// Gets or inits the provider selection options
        /// </summary>
        public GetProviderUnion ProviderSelection { get; init; }

        /// <summary>
        /// Gets or inits the beneficiary details
        /// </summary>
        public BeneficiaryUnion Beneficiary { get; init; }

        /// <summary>
        /// Gets or inits the retry flag object for the payment
        /// </summary>
        public BaseRetry? Retry { get; init; }
    }

    /// <summary>
    /// Defines a payment via Mandate
    /// </summary>
    [JsonDiscriminator("mandate")]
    public record Mandate : IDiscriminated
    {
        /// <summary>
        /// Gets the payment method type
        /// </summary>
        public string Type => "mandate";

        /// <summary>
        /// The identifier of the mandate
        /// </summary>
        public string MandateId { get; init; } = null!;

        /// <summary>
        /// The payment reference, useful for reconciliation needs
        /// </summary>
        public string? Reference { get; init; }

        /// <summary>
        /// The retry configuration
        /// </summary>
        public RetryUnion? Retry { get; init; }
    }
}
