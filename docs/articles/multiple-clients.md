# Multiple TrueLayer Clients

Configure and use multiple TrueLayer clients in the same application.

## Keyed Services (.NET 8+)

```csharp
services
    .AddKeyedTrueLayer("GBP", configuration, options =>
    {
        options.ClientId = "gbp-client-id";
        options.ClientSecret = "gbp-secret";
    })
    .AddKeyedTrueLayer("EUR", configuration, options =>
    {
        options.ClientId = "eur-client-id";
        options.ClientSecret = "eur-secret";
    });
```

## Dependency Injection

```csharp
public class PaymentService
{
    private readonly ITrueLayerClient _gbpClient;
    private readonly ITrueLayerClient _eurClient;

    public PaymentService(
        [FromKeyedServices("GBP")] ITrueLayerClient gbpClient,
        [FromKeyedServices("EUR")] ITrueLayerClient eurClient)
    {
        _gbpClient = gbpClient;
        _eurClient = eurClient;
    }
}
```

## See Also

- [Authentication](authentication.md)
- [Configuration](configuration.md)
