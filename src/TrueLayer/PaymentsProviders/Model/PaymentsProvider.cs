namespace TrueLayer.PaymentsProviders.Model;

/// <summary>
/// Represents a payments provider
/// </summary>
public record PaymentsProvider(
    string Id,
    Capabilities Capabilities,
    string? DisplayName = null,
    string? IconUri = null,
    string? LogoUri = null,
    string? BgColor = null,
    string? CountryCode = null
);