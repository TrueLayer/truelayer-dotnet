using System.Collections.Generic;
using OneOf;
using static TrueLayer.Payouts.Model.Beneficiary;

namespace TrueLayer.Payouts.Model
{
    using BeneficiaryUnion = OneOf<PaymentSource, ExternalAccount, BusinessAccount>;

    /// <summary>
    /// Represents a request for payout
    /// </summary>
    public record CreatePayoutRequest
    {
        /// <summary>
        /// Creates a new <see cref="CreatePayoutRequest"/>
        /// </summary>
        /// <param name="merchantAccountId">The merchant unique account ID</param>
        /// <param name="amountInMinor">The payout amount in the minor currency unit e.g. cents</param>
        /// <param name="currency">The three-letter ISO alpha currency code</param>
        /// <param name="beneficiary">The payout beneficiary details</param>
        /// <param name="metadata">Metadata</param>
        public CreatePayoutRequest(
            string merchantAccountId,
            long amountInMinor,
            string currency,
            BeneficiaryUnion beneficiary,
            Dictionary<string, string>? metadata = null)
        {
            MerchantAccountId = merchantAccountId;
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            Beneficiary = beneficiary;
            Metadata = metadata;
        }

        /// <summary>
        /// Gets the payout amount in the minor currency unit e.g. cents
        /// </summary>
        public string MerchantAccountId { get; }

        /// <summary>
        /// Gets the payout amount in the minor currency unit e.g. cents
        /// </summary>
        public long AmountInMinor { get; }

        /// <summary>
        /// Gets the three-letter ISO currency code
        /// </summary>
        /// <example>EUR</example>
        public string Currency { get; }
        
        /// <summary>
        /// Gets the beneficiary details
        /// </summary>
        public BeneficiaryUnion Beneficiary { get; }
        
        public Dictionary<string, string>? Metadata { get; }
    }
}
