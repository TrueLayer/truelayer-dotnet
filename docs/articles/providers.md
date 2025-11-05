# Payment Providers

Discover and retrieve information about payment providers (banks and financial institutions) that users can connect to for payments and mandates.

## Understanding Providers

Payment providers are the banks and financial institutions that TrueLayer connects to. Each provider has:
- **Capabilities**: What features they support (payments, payouts, VRP/mandates)
- **Logo and branding**: For display in your UI
- **Countries**: Which countries they operate in
- **Schemes**: Payment schemes they support (Faster Payments, SEPA, etc.)

## Listing All Providers

Retrieve all available payment providers using SearchPaymentsProvidersRequest:

```csharp
var searchRequest = new SearchPaymentsProvidersRequest(
    new AuthorizationFlow(new AuthorizationFlowConfiguration())
);

var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

if (response.IsSuccessful)
{
    foreach (var provider in response.Data.Items)
    {
        Console.WriteLine($"{provider.Id}: {provider.DisplayName}");
        Console.WriteLine($"  Country: {provider.CountryCode}");

        // Check payment capabilities
        if (provider.Capabilities.Payments?.BankTransfer != null)
        {
            Console.WriteLine($"  Supports Payments (Release: {provider.Capabilities.Payments.BankTransfer.ReleaseChannel})");
        }

        // Check mandate capabilities
        if (provider.Capabilities.Mandates?.VrpSweeping != null ||
            provider.Capabilities.Mandates?.VrpCommercial != null)
        {
            Console.WriteLine($"  Supports Variable Recurring Payments");
        }
    }
}
```

## Get Provider Details

Get detailed information about a specific provider:

```csharp
var response = await _client.PaymentsProviders.GetPaymentsProvider("mock-payments-gb-redirect");

if (response.IsSuccessful)
{
    var provider = response.Data;

    Console.WriteLine($"Provider: {provider.DisplayName}");
    Console.WriteLine($"ID: {provider.Id}");
    Console.WriteLine($"Country: {provider.CountryCode}");
    Console.WriteLine($"Logo: {provider.LogoUri}");
    Console.WriteLine($"Icon: {provider.IconUri}");
    Console.WriteLine($"Background Color: {provider.BgColor}");

    // Check payment capabilities
    if (provider.Capabilities.Payments?.BankTransfer != null)
    {
        Console.WriteLine("Supports bank transfer payments");
        Console.WriteLine($"  Release Channel: {provider.Capabilities.Payments.BankTransfer.ReleaseChannel}");
        Console.WriteLine($"  Schemes: {string.Join(", ", provider.Capabilities.Payments.BankTransfer.Schemes.Select(s => s.Id))}");
    }

    // Check mandate capabilities
    if (provider.Capabilities.Mandates?.VrpSweeping != null)
    {
        Console.WriteLine("Supports VRP Sweeping");
    }

    if (provider.Capabilities.Mandates?.VrpCommercial != null)
    {
        Console.WriteLine("Supports VRP Commercial");
    }
}
```

## Filtering Providers

### By Country

Filter providers by country code using the Countries parameter:

```csharp
var searchRequest = new SearchPaymentsProvidersRequest(
    new AuthorizationFlow(new AuthorizationFlowConfiguration()),
    Countries: new List<string> { "GB" }
);

var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

if (response.IsSuccessful)
{
    Console.WriteLine($"Found {response.Data.Items.Count} UK providers");

    foreach (var provider in response.Data.Items)
    {
        Console.WriteLine($"{provider.DisplayName} ({provider.Id})");
    }
}
```

### By Capability

Filter providers that support specific capabilities:

```csharp
public async Task<List<PaymentsProvider>> GetVRPProviders()
{
    // Filter for providers with VRP sweeping capabilities
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Capabilities: new Capabilities(
            Payments: null,
            Mandates: new MandatesCapabilities(
                VrpSweeping: new VrpSweepingCapabilities("general_availability"),
                VrpCommercial: null
            )
        )
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        throw new Exception("Failed to fetch providers");
    }

    return response.Data.Items.ToList();
}

public async Task<List<PaymentsProvider>> GetPaymentProviders()
{
    // Filter for providers with bank transfer payment capabilities
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Capabilities: new Capabilities(
            Payments: new PaymentsCapabilities(
                BankTransfer: new BankTransferCapabilities(
                    ReleaseChannel: "general_availability",
                    Schemes: new List<Scheme> { new Scheme("faster_payments_service") }
                )
            ),
            Mandates: null
        )
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        throw new Exception("Failed to fetch providers");
    }

    return response.Data.Items.ToList();
}
```

### By Multiple Criteria

Combine multiple filters:

```csharp
public async Task<List<PaymentsProvider>> GetUKPaymentProviders()
{
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Countries: new List<string> { "GB" },
        Currencies: new List<string> { "GBP" },
        ReleaseChannel: "general_availability",
        Capabilities: new Capabilities(
            Payments: new PaymentsCapabilities(
                BankTransfer: new BankTransferCapabilities(
                    ReleaseChannel: "general_availability",
                    Schemes: new List<Scheme> { new Scheme("faster_payments_service") }
                )
            ),
            Mandates: null
        )
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        return new List<PaymentsProvider>();
    }

    return response.Data.Items
        .OrderBy(p => p.DisplayName)
        .ToList();
}
```

## Provider Selection Patterns

### User Selection with Filtering

Allow users to choose from a filtered list of providers:

```csharp
public async Task<CreatePaymentRequest> CreatePaymentWithFilteredProviders(
    int amountInMinor,
    Beneficiary beneficiary,
    PaymentUserRequest userRequest)
{
    // Get UK providers that support payments using server-side filtering
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Countries: new List<string> { "GB" },
        Currencies: new List<string> { "GBP" },
        Capabilities: new Capabilities(
            Payments: new PaymentsCapabilities(
                BankTransfer: new BankTransferCapabilities(
                    ReleaseChannel: "general_availability",
                    Schemes: new List<Scheme> { new Scheme("faster_payments_service") }
                )
            ),
            Mandates: null
        )
    );

    var providersResponse = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!providersResponse.IsSuccessful)
    {
        throw new Exception("Failed to retrieve providers");
    }

    var ukProviders = providersResponse.Data.Items
        .Select(p => p.Id)
        .ToArray();

    return new CreatePaymentRequest(
        amountInMinor: amountInMinor,
        currency: Currencies.GBP,
        paymentMethod: new CreatePaymentMethod.BankTransfer(
            new CreateProviderSelection.UserSelected
            {
                Filter = new ProviderFilter
                {
                    ProviderIds = ukProviders
                }
            },
            beneficiary
        ),
        user: userRequest
    );
}
```

### Preselected Provider

Direct users to a specific provider (e.g., their bank):

```csharp
public async Task<CreatePaymentRequest> CreatePaymentWithPreselectedProvider(
    string providerId,
    int amountInMinor,
    Beneficiary beneficiary,
    PaymentUserRequest userRequest)
{
    // Verify provider exists and supports payments
    var providerResponse = await _client.PaymentsProviders.GetPaymentsProvider(providerId);

    if (!providerResponse.IsSuccessful)
    {
        throw new Exception($"Provider {providerId} not found");
    }

    if (providerResponse.Data.Capabilities.Payments?.BankTransfer == null)
    {
        throw new Exception($"Provider {providerId} does not support bank transfer payments");
    }

    return new CreatePaymentRequest(
        amountInMinor: amountInMinor,
        currency: Currencies.GBP,
        paymentMethod: new CreatePaymentMethod.BankTransfer(
            new CreateProviderSelection.Preselected(
                providerId: providerId,
                schemeSelection: new SchemeSelection.Preselected
                {
                    SchemeId = "faster_payments_service"
                }
            ),
            beneficiary
        ),
        user: userRequest
    );
}
```

## Building a Provider Selection UI

### Display Providers with Logos

```csharp
public class ProviderViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string IconUrl { get; set; }
    public string BackgroundColor { get; set; }
    public bool SupportsPayments { get; set; }
    public bool SupportsVRP { get; set; }
}

public async Task<List<ProviderViewModel>> GetProvidersForUI(string countryCode)
{
    // Use server-side filtering for country
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Countries: new List<string> { countryCode }
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        _logger.LogError("Failed to fetch providers: {TraceId}", response.TraceId);
        return new List<ProviderViewModel>();
    }

    return response.Data.Items
        .Select(p => new ProviderViewModel
        {
            Id = p.Id,
            Name = p.DisplayName,
            LogoUrl = p.LogoUri?.ToString(),
            IconUrl = p.IconUri?.ToString(),
            BackgroundColor = p.BgColor,
            SupportsPayments = p.Capabilities.Payments?.BankTransfer != null,
            SupportsVRP = p.Capabilities.Mandates?.VrpSweeping != null ||
                          p.Capabilities.Mandates?.VrpCommercial != null
        })
        .OrderBy(p => p.Name)
        .ToList();
}
```

### Grouped Provider Display

Group providers by various criteria:

```csharp
public class ProviderGroup
{
    public string Category { get; set; }
    public List<PaymentsProvider> Providers { get; set; }
}

public async Task<List<ProviderGroup>> GetGroupedProviders()
{
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration())
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        return new List<ProviderGroup>();
    }

    var groups = new List<ProviderGroup>();

    // Group by country
    var providersByCountry = response.Data.Items
        .GroupBy(p => p.CountryCode)
        .OrderBy(g => g.Key);

    foreach (var countryGroup in providersByCountry)
    {
        groups.Add(new ProviderGroup
        {
            Category = $"Providers in {countryGroup.Key}",
            Providers = countryGroup.OrderBy(p => p.DisplayName).ToList()
        });
    }

    return groups;
}

// Alternative: Group by capability using separate API calls
public async Task<List<ProviderGroup>> GetProvidersByCapability()
{
    var groups = new List<ProviderGroup>();

    // Get VRP-capable providers using server-side filtering
    var vrpRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Capabilities: new Capabilities(
            Payments: null,
            Mandates: new MandatesCapabilities(
                VrpSweeping: new VrpSweepingCapabilities("general_availability"),
                VrpCommercial: null
            )
        )
    );

    var vrpResponse = await _client.PaymentsProviders.SearchPaymentsProviders(vrpRequest);

    if (vrpResponse.IsSuccessful && vrpResponse.Data.Items.Any())
    {
        groups.Add(new ProviderGroup
        {
            Category = "Supports Variable Recurring Payments",
            Providers = vrpResponse.Data.Items.ToList()
        });
    }

    // Get payment-only providers using server-side filtering
    var paymentsRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Capabilities: new Capabilities(
            Payments: new PaymentsCapabilities(
                BankTransfer: new BankTransferCapabilities(
                    ReleaseChannel: "general_availability",
                    Schemes: new List<Scheme> { new Scheme("faster_payments_service") }
                )
            ),
            Mandates: null
        )
    );

    var paymentsResponse = await _client.PaymentsProviders.SearchPaymentsProviders(paymentsRequest);

    if (paymentsResponse.IsSuccessful && paymentsResponse.Data.Items.Any())
    {
        // Filter out providers that also have VRP capabilities (to get payments-only)
        var vrpProviderIds = vrpResponse.IsSuccessful
            ? vrpResponse.Data.Items.Select(p => p.Id).ToHashSet()
            : new HashSet<string>();

        var paymentOnlyProviders = paymentsResponse.Data.Items
            .Where(p => !vrpProviderIds.Contains(p.Id))
            .ToList();

        if (paymentOnlyProviders.Any())
        {
            groups.Add(new ProviderGroup
            {
                Category = "Payments Only",
                Providers = paymentOnlyProviders
            });
        }
    }

    return groups;
}
```

## Caching Provider Data

Cache provider information to reduce API calls:

```csharp
public class ProviderCache
{
    private readonly ITrueLayerClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProviderCache> _logger;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public ProviderCache(
        ITrueLayerClient client,
        IMemoryCache cache,
        ILogger<ProviderCache> logger)
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<PaymentsProvider>> GetProviders()
    {
        const string cacheKey = "truelayer_providers";

        if (_cache.TryGetValue(cacheKey, out List<PaymentsProvider> cached))
        {
            return cached;
        }

        var searchRequest = new SearchPaymentsProvidersRequest(
            new AuthorizationFlow(new AuthorizationFlowConfiguration())
        );

        var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

        if (!response.IsSuccessful)
        {
            _logger.LogWarning(
                "Failed to fetch providers: {TraceId}",
                response.TraceId
            );

            // Return empty list or throw exception
            return new List<PaymentsProvider>();
        }

        var providers = response.Data.Items.ToList();

        _cache.Set(cacheKey, providers, CacheDuration);

        return providers;
    }

    public async Task<PaymentsProvider?> GetProvider(string providerId)
    {
        var providers = await GetProviders();
        return providers.FirstOrDefault(p => p.Id == providerId);
    }
}
```

## Best Practices

### 1. Cache Provider Data

Provider data rarely changes - cache it for at least 24 hours:

```csharp
// Register cache in DI
services.AddMemoryCache();
services.AddSingleton<ProviderCache>();
```

### 2. Handle Provider Availability

Providers may be temporarily unavailable:

```csharp
public async Task<bool> IsProviderAvailable(string providerId)
{
    var response = await _client.PaymentsProviders.GetPaymentsProvider(providerId);

    if (!response.IsSuccessful)
    {
        return false;
    }

    // Additional checks for provider status if available
    // Check if provider supports required capability
    return response.Data.Capabilities.Payments?.BankTransfer != null;
}
```

### 3. Validate Provider Before Use

Always verify a provider exists before creating a payment:

```csharp
public async Task<ApiResponse<CreatePaymentResponse>> CreatePaymentSafely(
    string providerId,
    CreatePaymentRequest request)
{
    // Verify provider exists
    var providerResponse = await _client.PaymentsProviders.GetPaymentsProvider(providerId);

    if (!providerResponse.IsSuccessful)
    {
        _logger.LogWarning(
            "Provider {ProviderId} not found or unavailable",
            providerId
        );

        // Return error or fallback to user selection
        throw new InvalidOperationException($"Provider {providerId} is not available");
    }

    // Create payment
    return await _client.Payments.CreatePayment(
        request,
        idempotencyKey: Guid.NewGuid().ToString()
    );
}
```

### 4. Display Provider Logos

Use provider logos for better UX:

```csharp
// In your view model
public string GetProviderLogoUrl(PaymentsProvider provider)
{
    // Use logo for large displays, icon for small displays
    return provider.LogoUri?.ToString() ?? provider.IconUri?.ToString();
}

// HTML example
// <img src="@provider.LogoUrl" alt="@provider.Name" style="background-color: @provider.BackgroundColor" />
```

### 5. Filter by User's Country

Show only relevant providers to users:

```csharp
public async Task<List<PaymentsProvider>> GetProvidersForUser(User user)
{
    // Use server-side filtering by user's country
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Countries: new List<string> { user.CountryCode }
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        return new List<PaymentsProvider>();
    }

    return response.Data.Items
        .OrderBy(p => p.DisplayName)
        .ToList();
}
```

## Common Scenarios

### Bank Selection Page

```csharp
[HttpGet("select-bank")]
public async Task<IActionResult> SelectBank(string paymentId)
{
    // Use server-side filtering to get UK payment providers
    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Countries: new List<string> { "GB" },
        Currencies: new List<string> { "GBP" },
        Capabilities: new Capabilities(
            Payments: new PaymentsCapabilities(
                BankTransfer: new BankTransferCapabilities(
                    ReleaseChannel: "general_availability",
                    Schemes: new List<Scheme> { new Scheme("faster_payments_service") }
                )
            ),
            Mandates: null
        )
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        // Handle error appropriately
        return View(new BankSelectionViewModel
        {
            PaymentId = paymentId,
            Providers = new List<ProviderViewModel>()
        });
    }

    var ukProviders = response.Data.Items
        .Select(p => new ProviderViewModel
        {
            Id = p.Id,
            Name = p.DisplayName,
            LogoUrl = p.LogoUri?.ToString(),
            BackgroundColor = p.BgColor
        })
        .ToList();

    return View(new BankSelectionViewModel
    {
        PaymentId = paymentId,
        Providers = ukProviders
    });
}
```

### Dynamic Provider Filtering

Filter providers based on transaction requirements:

```csharp
public async Task<List<PaymentsProvider>> GetProvidersForTransaction(
    string countryCode,
    string currency,
    bool requiresVRP,
    int amountInMinor)
{
    // Build capabilities filter based on requirements
    Capabilities? capabilities = null;

    if (requiresVRP)
    {
        capabilities = new Capabilities(
            Payments: null,
            Mandates: new MandatesCapabilities(
                VrpSweeping: new VrpSweepingCapabilities("general_availability"),
                VrpCommercial: null
            )
        );
    }
    else
    {
        capabilities = new Capabilities(
            Payments: new PaymentsCapabilities(
                BankTransfer: new BankTransferCapabilities(
                    ReleaseChannel: "general_availability",
                    Schemes: new List<Scheme> { new Scheme("faster_payments_service") }
                )
            ),
            Mandates: null
        );
    }

    var searchRequest = new SearchPaymentsProvidersRequest(
        new AuthorizationFlow(new AuthorizationFlowConfiguration()),
        Countries: new List<string> { countryCode },
        Currencies: new List<string> { currency },
        ReleaseChannel: "general_availability",
        Capabilities: capabilities
    );

    var response = await _client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

    if (!response.IsSuccessful)
    {
        return new List<PaymentsProvider>();
    }

    // Could add additional client-side filtering based on amount limits, etc.
    return response.Data.Items
        .OrderBy(p => p.DisplayName)
        .ToList();
}
```

## See Also

- [Payments](payments.md) - Using providers in payment creation
- [Mandates](mandates.md) - Providers that support VRP/mandates
- [API Reference](xref:TrueLayer.PaymentsProviders.IPaymentsProvidersApi)
