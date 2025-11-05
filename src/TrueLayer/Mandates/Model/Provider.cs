using TrueLayer.Payments.Model;
using TrueLayer.Serialization;

namespace TrueLayer.Mandates.Model;

/// <summary>
/// Contains provider selection types for mandates.
/// </summary>
public static class Provider
{
    /// <summary>
    /// Provider Selection
    /// </summary>
    /// <param name="Type">The type of provider.</param>
    /// <param name="ProviderId">The provider Id the PSU will use for this payment.</param>
    /// <param name="Remitter">Remitter</param>
    [JsonDiscriminator("preselected")]
    public record Preselected(string Type, string ProviderId, RemitterAccount? Remitter = null) : IDiscriminated;
}