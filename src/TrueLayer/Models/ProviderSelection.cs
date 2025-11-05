namespace TrueLayer.Models;

/// <summary>
/// Represents the configuration status for provider selection UI rendering.
/// </summary>
public enum ConfigurationStatus
{
    /// <summary>
    /// Provider selection UI is supported and can be rendered.
    /// </summary>
    Supported = 0,

    /// <summary>
    /// Provider selection UI is not supported.
    /// </summary>
    NotSupported = 1
};

/// <summary>
/// Can the UI render a provider selection screen?
/// </summary>
/// <param name="Status"></param>
public record ProviderSelection(ConfigurationStatus Status);