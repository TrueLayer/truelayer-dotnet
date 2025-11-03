namespace TrueLayer.Models;

/// <summary>
/// Configuration object.
/// </summary>
/// <param name="ProviderSelection">Can the UI render a provider selection screen?</param>
/// <param name="Redirect">Can the UI redirect the end user to a third-party page?</param>
public record Configuration(ProviderSelection ProviderSelection, RedirectStatus Redirect);