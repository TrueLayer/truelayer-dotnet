using System.Collections.Generic;

namespace TrueLayer.PaymentsProviders.Model;

public record SearchPaymentsProvidersRequest(AuthorizationFlow AuthorizationFlow,
    List<string>? Countries = null,
    List<string>? Currencies = null,
    string? ReleaseChannel = null,
    List<string>? CustomerSegments = null,
    Capabilities? Capabilities = null);
