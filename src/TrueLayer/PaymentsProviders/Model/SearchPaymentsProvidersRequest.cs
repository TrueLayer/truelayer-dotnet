using System.Collections.Generic;

namespace TrueLayer.PaymentsProviders.Model;

/// <summary>
/// Represents a request to search for payment providers matching specific criteria.
/// </summary>
/// <param name="AuthorizationFlow">The authorization flow configuration required.</param>
/// <param name="Countries">Optional list of country codes to filter providers by.</param>
/// <param name="Currencies">Optional list of currencies to filter providers by.</param>
/// <param name="ReleaseChannel">Optional release channel filter.</param>
/// <param name="CustomerSegments">Optional list of customer segments to filter by.</param>
/// <param name="Capabilities">Optional capabilities filter for providers.</param>
public record SearchPaymentsProvidersRequest(AuthorizationFlow AuthorizationFlow,
    List<string>? Countries = null,
    List<string>? Currencies = null,
    string? ReleaseChannel = null,
    List<string>? CustomerSegments = null,
    Capabilities? Capabilities = null);
