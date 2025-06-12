using TrueLayer.Common;

namespace TrueLayer.Payouts.Model
{
    /// <summary>
    /// Represents sub-merchant details for payouts
    /// </summary>
    public record PayoutSubMerchants
    {
        /// <summary>
        /// Creates a new <see cref="PayoutSubMerchants"/>
        /// </summary>
        /// <param name="ultimateCounterparty">The ultimate counterparty details (optional for payouts)</param>
        public PayoutSubMerchants(UltimateCounterpartyBusinessClient? ultimateCounterparty = null)
        {
            UltimateCounterparty = ultimateCounterparty;
        }

        /// <summary>
        /// Gets the ultimate counterparty details (only business_client is supported for payouts)
        /// </summary>
        public UltimateCounterpartyBusinessClient? UltimateCounterparty { get; }
    }
}