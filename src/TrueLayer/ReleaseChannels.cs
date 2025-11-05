namespace TrueLayer;

/// <summary>
/// Represents the release stage of a provider
/// </summary>
public static class ReleaseChannels
{
    /// <summary>
    /// Represents a provider in private beta release stage.
    /// </summary>
    public const string PrivateBeta = "private_beta";

    /// <summary>
    /// Represents a provider in public beta release stage.
    /// </summary>
    public const string PublicBeta = "public_beta";

    /// <summary>
    /// Represents a provider in general availability release stage.
    /// </summary>
    public const string GeneralAvailability = "general_availability";
}