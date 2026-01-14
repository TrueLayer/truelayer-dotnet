# Authentication

The TrueLayer .NET client handles authentication automatically using your Client ID and Secret. However, you can optimize performance using auth token caching strategies.

## Basic Configuration

Configure authentication in your `appsettings.json`:

```json
{
  "TrueLayer": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "UseSandbox": true
  }
}
```

## Token Caching Strategies

Auth tokens have a limited lifetime. Caching them reduces API calls and improves performance.

### In-Memory Caching

Recommended for most applications:

```csharp
services.AddTrueLayer(
    configuration,
    options => { /* config */ },
    authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory
);
```

### Custom Caching

Implement `IAuthTokenCache` for distributed caching:

```csharp
public class RedisAuthTokenCache : IAuthTokenCache
{
    private readonly IDistributedCache _cache;

    public RedisAuthTokenCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string?> GetAsync(string key, CancellationToken ct = default)
    {
        return await _cache.GetStringAsync(key, ct);
    }

    public async Task SetAsync(
        string key,
        string value,
        TimeSpan expiry,
        CancellationToken ct = default)
    {
        await _cache.SetStringAsync(
            key,
            value,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry },
            ct
        );
    }
}

// Register
services.AddSingleton<IAuthTokenCache, RedisAuthTokenCache>();
services.AddTrueLayer(
    configuration,
    options => { /* config */ },
    authTokenCachingStrategy: AuthTokenCachingStrategies.Custom
);
```

## Environment Configuration

### Sandbox vs Production

Control which environment to use:

```csharp
// Use sandbox
"TrueLayer": {
    "UseSandbox": true
}

// Use production
"TrueLayer": {
    "UseSandbox": false
}
```

### Custom API URIs

Override default endpoints:

```csharp
services.AddTrueLayer(configuration, options =>
{
    options.Auth = new ApiOptions
    {
        Uri = new Uri("https://custom-auth.truelayer.com")
    };
});
```

## Signing Keys for Payments

Payment requests must be cryptographically signed:

```csharp
services.AddTrueLayer(configuration, options =>
{
    if (options.Payments?.SigningKey != null)
    {
        // Load from secure storage (e.g., Azure Key Vault, AWS Secrets Manager)
        options.Payments.SigningKey.PrivateKey = await secretManager.GetSecretAsync("truelayer-private-key");
    }
});
```

> [!WARNING]
> Never hardcode private keys or commit them to source control.

## Multiple Clients

Configure multiple clients with different credentials:

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

Usage:

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

## Security Best Practices

### 1. Secure Credential Storage

Use secret management services:

```csharp
// Azure Key Vault
var keyVault = new SecretClient(
    new Uri("https://your-keyvault.vault.azure.net/"),
    new DefaultAzureCredential()
);

var clientSecret = await keyVault.GetSecretAsync("TrueLayer-ClientSecret");
var privateKey = await keyVault.GetSecretAsync("TrueLayer-PrivateKey");

services.AddTrueLayer(configuration, options =>
{
    options.ClientSecret = clientSecret.Value.Value;
    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.PrivateKey = privateKey.Value.Value;
    }
});
```

### 2. Rotate Credentials Regularly

Implement credential rotation:

```csharp
services.AddOptions<TrueLayerOptions>()
    .Configure<ISecretManager>((options, secretManager) =>
    {
        // Reload credentials periodically
        options.ClientSecret = secretManager.GetSecret("ClientSecret");
    });
```

### 3. Limit Token Lifetime

Configure shorter cache durations for sensitive environments:

```csharp
services.AddSingleton<IAuthTokenCache>(sp =>
    new CustomAuthTokenCache(maxLifetime: TimeSpan.FromMinutes(30))
);
```

### 4. Use HTTPS Only

Ensure all traffic is encrypted (default behavior).

## Troubleshooting

### 401 Unauthorized

- Verify Client ID and Secret are correct
- Check credentials are for the correct environment (sandbox/production)
- Ensure credentials haven't expired

### 403 Forbidden

- Verify your application has the required API permissions in TrueLayer Console
- Check signing key is uploaded and Key ID is correct

### Token Cache Issues

- Clear cache and retry
- Verify cache implementation is working correctly
- Check cache expiry times

## See Also

- [Installation](installation.md)
- [Multiple Clients](multiple-clients.md)
- [Configuration Reference](configuration.md)
