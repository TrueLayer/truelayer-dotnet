# Configuration

Advanced configuration options for customizing the TrueLayer .NET client to suit your application's needs.

## TrueLayerOptions Overview

The `TrueLayerOptions` class provides comprehensive configuration for the TrueLayer client:

```csharp
public class TrueLayerOptions
{
    // Required: Your TrueLayer credentials
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    // Environment selection
    public bool UseSandbox { get; set; } = false;

    // API endpoints (optional - uses defaults if not set)
    public ApiOptions Auth { get; set; }
    public PaymentsOptions Payments { get; set; }
    public ApiOptions MerchantAccounts { get; set; }
    public ApiOptions Payouts { get; set; }
}
```

For more details, see the [TrueLayer API documentation](https://docs.truelayer.com/docs).

## Basic Configuration

### Configuration from appsettings.json

The simplest configuration using `appsettings.json`:

```json
{
  "TrueLayer": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "UseSandbox": true,
    "Payments": {
      "SigningKey": {
        "KeyId": "your-key-id"
      }
    }
  }
}
```

```csharp
services.AddTrueLayer(configuration, options =>
{
    if (options.Payments?.SigningKey != null)
    {
        // Load private key from file
        options.Payments.SigningKey.PrivateKey = File.ReadAllText("ec512-private-key.pem");
    }
});
```

### Manual Configuration

Configure options programmatically:

```csharp
services.AddTrueLayer(configuration, options =>
{
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.UseSandbox = true;

    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.KeyId = "your-key-id";
        options.Payments.SigningKey.PrivateKey = privateKeyContent;
    }
});
```

## Environment Configuration

### Sandbox vs Production

Switch between sandbox and production environments:

```csharp
public static class TrueLayerConfiguration
{
    public static IServiceCollection ConfigureTrueLayer(
        this IServiceCollection services,
        IConfiguration configuration,
        bool useSandbox)
    {
        services.AddTrueLayer(configuration, options =>
        {
            options.UseSandbox = useSandbox;

            if (options.Payments?.SigningKey != null)
            {
                // Use different keys for different environments
                var keyFile = useSandbox
                    ? "ec512-sandbox-private-key.pem"
                    : "ec512-production-private-key.pem";

                options.Payments.SigningKey.PrivateKey = File.ReadAllText(keyFile);
            }
        });

        return services;
    }
}

// Usage
if (builder.Environment.IsDevelopment())
{
    services.ConfigureTrueLayer(configuration, useSandbox: true);
}
else
{
    services.ConfigureTrueLayer(configuration, useSandbox: false);
}
```

### Environment-Specific Configuration

Use different configurations per environment:

```csharp
// appsettings.Development.json
{
  "TrueLayer": {
    "UseSandbox": true,
    "ClientId": "sandbox-client-id",
    "ClientSecret": "sandbox-client-secret"
  }
}

// appsettings.Production.json
{
  "TrueLayer": {
    "UseSandbox": false,
    "ClientId": "production-client-id",
    "ClientSecret": "production-client-secret"
  }
}
```

## Custom API Endpoints

### Override Default Endpoints

Configure custom API endpoints (useful for testing or proxying):

```csharp
services.AddTrueLayer(configuration, options =>
{
    // Custom auth endpoint
    options.Auth = new ApiOptions
    {
        Uri = new Uri("https://custom-auth.truelayer.com")
    };

    // Custom payments endpoint
    if (options.Payments != null)
    {
        options.Payments.Uri = new Uri("https://custom-api.truelayer.com");
    }

    // Custom merchant accounts endpoint
    options.MerchantAccounts = new ApiOptions
    {
        Uri = new Uri("https://custom-api.truelayer.com")
    };

    // Custom payouts endpoint
    options.Payouts = new ApiOptions
    {
        Uri = new Uri("https://custom-api.truelayer.com")
    };
});
```

### Proxy Configuration

Route requests through a proxy:

```csharp
services.AddTrueLayer(configuration, options =>
{
    options.UseSandbox = true;

    // Configure all endpoints through proxy
    var proxyUri = new Uri("https://proxy.yourdomain.com");

    options.Auth = new ApiOptions { Uri = proxyUri };
    if (options.Payments != null)
    {
        options.Payments.Uri = proxyUri;
    }
    options.MerchantAccounts = new ApiOptions { Uri = proxyUri };
    options.Payouts = new ApiOptions { Uri = proxyUri };
});
```

## HTTP Client Configuration

### Custom HttpClient

Configure the underlying HttpClient for advanced scenarios:

```csharp
services.AddTrueLayer(configuration, options =>
{
    // Configuration happens here
},
authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory,
configureHttpClient: (httpClient) =>
{
    // Custom timeout
    httpClient.Timeout = TimeSpan.FromSeconds(60);

    // Custom headers
    httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "CustomValue");

    // User agent
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");
});
```

### Multiple Named HttpClients

Use named HttpClients for different scenarios:

```csharp
// Configure default client with shorter timeout for quick operations
services.AddTrueLayer(configuration, options => { },
    authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory,
    configureHttpClient: client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });

// Configure a separate client for long-running operations
services.AddKeyedTrueLayer("long-running", configuration, options => { },
    authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory,
    configureHttpClient: client =>
    {
        client.Timeout = TimeSpan.FromMinutes(2);
    });
```

### Retry Policies

Configure retry behavior using Polly (requires `Microsoft.Extensions.Http.Polly`):

```csharp
using Polly;
using Polly.Extensions.Http;

services.AddTrueLayer(configuration, options => { });

// Add retry policy to the named HttpClient
services.AddHttpClient(TrueLayerClient.HttpClientName)
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

## Signing Key Configuration

### Loading from File

Load private key from a PEM file:

```csharp
services.AddTrueLayer(configuration, options =>
{
    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.KeyId = configuration["TrueLayer:Payments:SigningKey:KeyId"];
        options.Payments.SigningKey.PrivateKey = File.ReadAllText("ec512-private-key.pem");
    }
});
```

### Loading from Azure Key Vault

Load private key from Azure Key Vault:

```csharp
services.AddTrueLayer(configuration, async options =>
{
    if (options.Payments?.SigningKey != null)
    {
        var keyVaultClient = new SecretClient(
            new Uri("https://your-keyvault.vault.azure.net/"),
            new DefaultAzureCredential()
        );

        var secret = await keyVaultClient.GetSecretAsync("truelayer-signing-key");

        options.Payments.SigningKey.KeyId = configuration["TrueLayer:Payments:SigningKey:KeyId"];
        options.Payments.SigningKey.PrivateKey = secret.Value.Value;
    }
});
```

### Loading from AWS Secrets Manager

Load private key from AWS Secrets Manager:

```csharp
services.AddTrueLayer(configuration, async options =>
{
    if (options.Payments?.SigningKey != null)
    {
        var client = new AmazonSecretsManagerClient(RegionEndpoint.EUWest1);

        var request = new GetSecretValueRequest
        {
            SecretId = "truelayer-signing-key"
        };

        var response = await client.GetSecretValueAsync(request);

        options.Payments.SigningKey.KeyId = configuration["TrueLayer:Payments:SigningKey:KeyId"];
        options.Payments.SigningKey.PrivateKey = response.SecretString;
    }
});
```

### Loading from Environment Variables

Load from environment variables:

```csharp
services.AddTrueLayer(configuration, options =>
{
    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.KeyId = Environment.GetEnvironmentVariable("TRUELAYER_KEY_ID");
        options.Payments.SigningKey.PrivateKey = Environment.GetEnvironmentVariable("TRUELAYER_PRIVATE_KEY");
    }
});
```

## Authentication Token Caching

### In-Memory Caching

Use built-in in-memory caching:

```csharp
services.AddTrueLayer(
    configuration,
    options => { },
    authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory
);
```

### Distributed Caching

Implement custom caching with Redis or other distributed cache:

```csharp
public class DistributedAuthTokenCache : IAuthTokenCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedAuthTokenCache> _logger;

    public DistributedAuthTokenCache(
        IDistributedCache cache,
        ILogger<DistributedAuthTokenCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<AuthToken?> GetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await _cache.GetStringAsync("truelayer_auth_token", cancellationToken);

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<AuthToken>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving auth token from cache");
            return null;
        }
    }

    public async Task SetAsync(AuthToken token, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(token);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.ExpiresIn - 60)
            };

            await _cache.SetStringAsync("truelayer_auth_token", json, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing auth token in cache");
        }
    }
}

// Register
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
});

services.AddSingleton<IAuthTokenCache, DistributedAuthTokenCache>();

services.AddTrueLayer(
    configuration,
    options => { },
    authTokenCachingStrategy: AuthTokenCachingStrategies.Custom
);
```

For more details, see [Authentication](authentication.md).

## Multiple Client Configuration

### Named Clients

Configure multiple TrueLayer clients for different purposes:

```csharp
// Primary client for customer transactions
services.AddTrueLayer(configuration, options =>
{
    options.ClientId = configuration["TrueLayer:Primary:ClientId"];
    options.ClientSecret = configuration["TrueLayer:Primary:ClientSecret"];
    options.UseSandbox = false;
});

// Secondary client for internal operations
services.AddKeyedTrueLayer("internal", configuration, options =>
{
    options.ClientId = configuration["TrueLayer:Internal:ClientId"];
    options.ClientSecret = configuration["TrueLayer:Internal:ClientSecret"];
    options.UseSandbox = true;
});

// Usage
public class PaymentService
{
    private readonly ITrueLayerClient _primaryClient;
    private readonly ITrueLayerClient _internalClient;

    public PaymentService(
        ITrueLayerClient primaryClient,
        [FromKeyedServices("internal")] ITrueLayerClient internalClient)
    {
        _primaryClient = primaryClient;
        _internalClient = internalClient;
    }
}
```

For more details, see [Multiple Clients](multiple-clients.md).

## Logging Configuration

### Enable HTTP Logging

Enable detailed HTTP request/response logging:

```csharp
services.AddTrueLayer(configuration, options => { });

// Add HTTP logging
services.AddHttpClient(TrueLayerClient.HttpClientName)
    .AddHttpMessageHandler(() => new LoggingHandler());

public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request: {Method} {Uri}", request.Method, request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);

        _logger.LogInformation(
            "Response: {StatusCode} for {Method} {Uri}",
            response.StatusCode,
            request.Method,
            request.RequestUri
        );

        return response;
    }
}
```

### Configure Logging Levels

Configure logging levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "TrueLayer": "Debug",
      "System.Net.Http": "Warning"
    }
  }
}
```

## Security Best Practices

### Protect Credentials

Never commit credentials to source control:

```csharp
// ❌ Bad - hardcoded credentials
options.ClientId = "hardcoded-client-id";
options.ClientSecret = "hardcoded-secret";

// ✅ Good - from configuration
options.ClientId = configuration["TrueLayer:ClientId"];
options.ClientSecret = configuration["TrueLayer:ClientSecret"];

// ✅ Better - from secure storage (Key Vault, Secrets Manager)
options.ClientSecret = await GetSecretFromVault("truelayer-client-secret");
```

### Validate Configuration

Validate configuration on startup:

```csharp
public static class TrueLayerConfigurationValidator
{
    public static void ValidateConfiguration(TrueLayerOptions options)
    {
        if (string.IsNullOrEmpty(options.ClientId))
        {
            throw new InvalidOperationException("TrueLayer ClientId is required");
        }

        if (string.IsNullOrEmpty(options.ClientSecret))
        {
            throw new InvalidOperationException("TrueLayer ClientSecret is required");
        }

        if (options.Payments?.SigningKey != null)
        {
            if (string.IsNullOrEmpty(options.Payments.SigningKey.KeyId))
            {
                throw new InvalidOperationException("Payments SigningKey KeyId is required");
            }

            if (string.IsNullOrEmpty(options.Payments.SigningKey.PrivateKey))
            {
                throw new InvalidOperationException("Payments SigningKey PrivateKey is required");
            }
        }
    }
}

// Use in Startup
services.AddTrueLayer(configuration, options =>
{
    // Configure options...
    TrueLayerConfigurationValidator.ValidateConfiguration(options);
});
```

## Configuration Examples

### Minimal Production Configuration

```csharp
services.AddTrueLayer(configuration, options =>
{
    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.PrivateKey = File.ReadAllText("ec512-private-key.pem");
    }
},
authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory);
```

### Advanced Production Configuration

```csharp
services.AddTrueLayer(configuration, async options =>
{
    // Load credentials from Key Vault
    var keyVault = new SecretClient(
        new Uri(configuration["KeyVault:Uri"]),
        new DefaultAzureCredential()
    );

    options.ClientSecret = (await keyVault.GetSecretAsync("truelayer-client-secret")).Value.Value;

    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.PrivateKey =
            (await keyVault.GetSecretAsync("truelayer-private-key")).Value.Value;
    }
},
authTokenCachingStrategy: AuthTokenCachingStrategies.Custom,
configureHttpClient: client =>
{
    client.Timeout = TimeSpan.FromSeconds(45);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0.0");
});

// Configure distributed caching
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
});

// Configure retry policy
services.AddHttpClient(TrueLayerClient.HttpClientName)
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

## Troubleshooting

### Configuration Validation Errors

If you see configuration errors on startup:

```csharp
// Add diagnostics
services.PostConfigure<TrueLayerOptions>(options =>
{
    var logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<TrueLayerOptions>();

    logger.LogInformation("TrueLayer Configuration:");
    logger.LogInformation("  ClientId: {ClientId}", options.ClientId?.Substring(0, 8) + "...");
    logger.LogInformation("  UseSandbox: {UseSandbox}", options.UseSandbox);
    logger.LogInformation("  HasSigningKey: {HasKey}", options.Payments?.SigningKey?.PrivateKey != null);
});
```

### Connection Issues

If experiencing connection issues:

```csharp
// Increase timeout
services.AddTrueLayer(configuration, options => { },
    configureHttpClient: client =>
    {
        client.Timeout = TimeSpan.FromSeconds(120);
    });

// Add diagnostic logging
services.AddHttpClient(TrueLayerClient.HttpClientName)
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(120);
    })
    .AddHttpMessageHandler(() => new DiagnosticHandler());
```

## See Also

- [Installation](installation.md) - Getting started with TrueLayer
- [Authentication](authentication.md) - Authentication and token caching
- [Multiple Clients](multiple-clients.md) - Using multiple TrueLayer clients
- [API Reference](xref:TrueLayer.TrueLayerOptions)
