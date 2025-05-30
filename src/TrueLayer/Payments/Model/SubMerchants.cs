using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Represents sub-merchant information for marketplace and platform payments
    /// </summary>
    public record SubMerchants
    {
        /// <summary>
        /// Creates a new <see cref="SubMerchants"/> instance
        /// </summary>
        /// <param name="ultimateCounterparty">The ultimate counterparty information</param>
        public SubMerchants(UltimateCounterparty? ultimateCounterparty = null)
        {
            UltimateCounterparty = ultimateCounterparty;
        }

        /// <summary>
        /// Gets the ultimate counterparty information
        /// </summary>
        public UltimateCounterparty? UltimateCounterparty { get; }
    }

    /// <summary>
    /// Represents the ultimate counterparty in a payment transaction
    /// </summary>
    [JsonDiscriminator("business_division")]
    public record UltimateCounterparty : IDiscriminated
    {
        /// <summary>
        /// Creates a new <see cref="UltimateCounterparty"/> instance with business division type
        /// </summary>
        public UltimateCounterparty()
        {
        }

        /// <summary>
        /// Gets the type of ultimate counterparty
        /// </summary>
        public string Type => "business_division";
    }
}