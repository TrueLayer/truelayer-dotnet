using TrueLayer.Serialization;

namespace TrueLayer.MerchantAccounts.Model;

/// <summary>
/// Contains types representing outbound transactions that a return payment is for.
/// </summary>
public static class ReturnFor
{
    /// <summary>
    /// Defines an identified outbound transaction.
    /// </summary>
    /// <param name="ReturnedId">Unique ID for the outbound transaction that returned</param>
    [JsonDiscriminator(Discriminator)]
    public record Identified(string ReturnedId) : IDiscriminated
    {
        const string Discriminator = "identified";

        /// <summary>
        /// Gets the type discriminator for identified return transactions.
        /// </summary>
        public string Type => Discriminator;
    }

    /// <summary>
    /// Defines an unknown outbound transaction.
    /// </summary>
    [JsonDiscriminator(Discriminator)]
    public record Unknown : IDiscriminated
    {
        const string Discriminator = "unknown";

        /// <summary>
        /// Gets the type discriminator for unknown return transactions.
        /// </summary>
        public string Type => Discriminator;
    }
}
