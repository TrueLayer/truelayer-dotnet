using TrueLayer.Serialization;
using OneOf;

namespace TrueLayer.MerchantAccounts.Model;

public static class ReturnFor
{
    /// <summary>
    /// Defines an identified outbound transaction.
    /// </summary>
    /// <param name="ReturnedId">Unique ID for the outbound transaction that returned</param>
    [JsonDiscriminator(Discriminator)]
    public record Idenfied(string ReturnedId) : IDiscriminated
    {
        const string Discriminator = "identified";
        public string Type => Discriminator;
    }

    /// <summary>
    /// Defines an unknown outbound transaction.
    /// </summary>
    [JsonDiscriminator(Discriminator)]
    public record Unknow : IDiscriminated
    {
        const string Discriminator = "unknown";
        public string Type => Discriminator;
    }
}
