namespace TrueLayer.Models;

/// <summary>
/// Submit the provider details selected by the PSU
/// </summary>
/// <param name="ProviderId"></param>
public record SubmitProviderSelectionRequest(string ProviderId);