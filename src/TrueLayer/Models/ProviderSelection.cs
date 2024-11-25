namespace TrueLayer.Models
{
    public enum ConfigurationStatus { Supported = 0, NotSupported = 1 };

    /// <summary>
    /// Can the UI render a provider selection screen?
    /// </summary>
    /// <param name="Status"></param>
    public record ProviderSelection(ConfigurationStatus Status);
}
