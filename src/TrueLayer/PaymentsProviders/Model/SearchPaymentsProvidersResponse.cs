using System.Collections.Generic;

namespace TrueLayer.PaymentsProviders.Model;

/// <summary>
/// Represents the response from searching for payment providers.
/// </summary>
/// <param name="Items">The list of payment providers matching the search criteria.</param>
public record SearchPaymentsProvidersResponse(List<PaymentsProvider> Items);
