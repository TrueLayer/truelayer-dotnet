# Payment Providers

Discover and retrieve information about payment providers.

## Get Provider Details

```csharp
var response = await _client.PaymentsProviders.GetPaymentsProvider("mock-payments-gb-redirect");

if (response.IsSuccessful)
{
    var provider = response.Data;
    Console.WriteLine($"Provider: {provider.DisplayName}");
    Console.WriteLine($"Logo: {provider.LogoUri}");
}
```

## See Also

- [API Reference](xref:TrueLayer.PaymentsProviders.IPaymentsProvidersApi)
