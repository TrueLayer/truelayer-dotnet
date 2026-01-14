# Installation

## Requirements

- .NET 9.0 or .NET 8.0
- A TrueLayer developer account

## Installing the Package

### Using .NET CLI

```bash
dotnet add package TrueLayer.Client
```

### Using Package Manager Console

```powershell
Install-Package TrueLayer.Client
```

### Using NuGet CLI

```bash
nuget install TrueLayer.Client
```

## Prerequisites

Before you can use the TrueLayer .NET client, you need to:

### 1. Create a TrueLayer Account

[Sign up](https://console.truelayer.com/) for a developer account and create a new application.

### 2. Obtain Credentials

From the TrueLayer Console, obtain:
- **Client ID**
- **Client Secret**

### 3. Generate Signing Keys

For payment requests, you need to generate EC512 signing keys:

```bash
# Generate private key
docker run --rm -v ${PWD}:/out -w /out -it alpine/openssl ecparam -genkey -name secp521r1 -noout -out ec512-private-key.pem

# Generate public key
docker run --rm -v ${PWD}:/out -w /out -it alpine/openssl ec -in ec512-private-key.pem -pubout -out ec512-public-key.pem
```

> [!WARNING]
> Store your private key securely. Never commit it to source control.

### 4. Upload Public Key

1. Navigate to Payments settings in the TrueLayer Console
2. Upload your public key (`ec512-public-key.pem`)
3. Note the generated Key ID

### 5. Configure Application

Add your credentials to `appsettings.json`:

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

### 6. Register Services

In your `Program.cs` or `Startup.cs`:

```csharp
services.AddTrueLayer(configuration, options =>
{
    if (options.Payments?.SigningKey != null)
    {
        // Load private key from secure storage
        options.Payments.SigningKey.PrivateKey = File.ReadAllText("ec512-private-key.pem");
    }
},
// Optional: Enable auth token caching for better performance
authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory);
```

## Verify Installation

Test your setup with a simple call:

```csharp
public class StartupTest
{
    private readonly ITrueLayerClient _client;

    public StartupTest(ITrueLayerClient client)
    {
        _client = client;
    }

    public async Task TestConnection()
    {
        // Try fetching a payment provider
        var response = await _client.PaymentsProviders.GetPaymentsProvider("mock-payments-gb-redirect");

        if (response.IsSuccessful)
        {
            Console.WriteLine($"Connected! Provider: {response.Data.DisplayName}");
        }
        else
        {
            Console.WriteLine($"Connection failed: {response.Problem}");
        }
    }
}
```

## Next Steps

- [Configure Authentication](authentication.md)
- [Create Your First Payment](payments.md)
- [Handle Errors](error-handling.md)
