using System.Collections.Generic;
using TrueLayer.Models;

namespace TrueLayer.PaymentsProviders.Model;

public record SearchPaymentsProvidersRequest(AuthorizationFlow AuthorizationFlow,
    List<string>? Countries = null,
    List<string>? Currencies = null,
    List<string>? ReleaseChannels = null,
    List<string>? CustomerSegments = null,
    Capabilities? Capabilities = null);
