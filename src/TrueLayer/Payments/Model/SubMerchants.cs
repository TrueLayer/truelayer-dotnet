using OneOf;
using TrueLayer.Common;

namespace TrueLayer.Payments.Model
{
    using PaymentUltimateCounterpartyUnion = OneOf<UltimateCounterpartyBusinessClient, UltimateCounterpartyBusinessDivision>;

    /// <summary>
    /// Represents sub-merchant details for payments
    /// </summary>
    public record PaymentSubMerchants
    {
        /// <summary>
        /// Creates a new <see cref="PaymentSubMerchants"/>
        /// </summary>
        /// <param name="ultimateCounterparty">The ultimate counterparty details</param>
        public PaymentSubMerchants(PaymentUltimateCounterpartyUnion ultimateCounterparty)
        {
            UltimateCounterparty = ultimateCounterparty;
        }

        /// <summary>
        /// Gets the ultimate counterparty details
        /// </summary>
        public PaymentUltimateCounterpartyUnion UltimateCounterparty { get; }
    }
}