# TrueLayer.NET Documentation

[![NuGet](https://img.shields.io/nuget/v/TrueLayer.Client.svg)](https://www.nuget.org/packages/TrueLayer.Client)
[![NuGet](https://img.shields.io/nuget/dt/TrueLayer.Client.svg)](https://www.nuget.org/packages/TrueLayer.Client)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://truelayer.mit-license.org/)
[![Build](https://github.com/TrueLayer/truelayer-dotnet/workflows/Build/badge.svg)](https://github.com/TrueLayer/truelayer-dotnet/actions)

Welcome to the official TrueLayer .NET client library documentation. This library provides convenient access to the TrueLayer APIs for applications built with .NET.

## What is TrueLayer?

[TrueLayer](https://truelayer.com) is a leading open banking platform that enables secure access to financial data and payment initiation services. Our APIs allow you to:

- **Initiate Payments** - Create and manage single and recurring payments
- **Process Payouts** - Send funds to beneficiaries
- **Manage Mandates** - Set up recurring payment mandates
- **Access Provider Information** - Discover available payment providers
- **Manage Merchant Accounts** - View and manage your merchant accounts

## Supported Frameworks

The library currently supports:
- **.NET 9.0**
- **.NET 8.0**

## Quick Start

### Installation

Install the library via NuGet:

```bash
dotnet add package TrueLayer.Client
```

### Basic Setup

1. [Sign up](https://console.truelayer.com/) for a TrueLayer developer account
2. Create an application and obtain your Client ID and Secret
3. Generate signing keys for payment requests
4. Configure your application:

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

### Your First Payment

```csharp
public class PaymentService
{
    private readonly ITrueLayerClient _client;

    public PaymentService(ITrueLayerClient client)
    {
        _client = client;
    }

    public async Task<string> CreatePayment()
    {
        var request = new CreatePaymentRequest(
            amountInMinor: 100,
            currency: Currencies.GBP,
            paymentMethod: new CreatePaymentMethod.BankTransfer(
                new CreateProviderSelection.UserSelected(),
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new AccountIdentifier.SortCodeAccountNumber("567890", "12345678")
                )
            ),
            user: new PaymentUserRequest("Jane Doe", "jane.doe@example.com")
        );

        var response = await _client.Payments.CreatePayment(
            request,
            idempotencyKey: Guid.NewGuid().ToString()
        );

        if (!response.IsSuccessful)
        {
            throw new Exception($"Payment creation failed: {response.Problem}");
        }

        return response.Data!.Match(
            authRequired => authRequired.HostedPage!.Uri.ToString(),
            authorized => $"Payment authorized: {authorized.Id}",
            failed => $"Payment failed: {failed.Status}",
            authorizing => $"Payment authorizing: {authorizing.Id}"
        );
    }
}
```

## Documentation Structure

### Getting Started
- [Installation](articles/installation.md) - Detailed installation and setup guide
- [Authentication](articles/authentication.md) - Configure authentication and token caching
- [Error Handling](articles/error-handling.md) - Handle errors and responses effectively

### Core Features
- [Payments](articles/payments.md) - Create and manage payments, refunds, and cancellations
- [Payouts](articles/payouts.md) - Process payouts to beneficiaries
- [Mandates](articles/mandates.md) - Set up and manage recurring payment mandates
- [Providers](articles/providers.md) - Discover and work with payment providers
- [Merchant Accounts](articles/merchant-accounts.md) - Manage your merchant accounts

### Advanced Topics
- [Multiple Clients](articles/multiple-clients.md) - Configure multiple TrueLayer clients
- [Custom Configuration](articles/configuration.md) - Advanced configuration options

### API Reference
Browse the complete **API Reference** in the navigation menu for detailed documentation of all types and members.

## Examples

Check out our [MVC Example](https://github.com/TrueLayer/truelayer-dotnet/tree/main/examples/MvcExample) for a complete working application demonstrating all major features.

## Getting Help

- **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/TrueLayer/truelayer-dotnet/issues)
- **API Reference**: Browse the complete [API documentation](https://docs.truelayer.com)
- **Support**: Contact [TrueLayer Support](https://truelayer.com/support)

## Contributing

We welcome contributions! Please see our [Contributing Guide](https://github.com/TrueLayer/truelayer-dotnet/blob/main/CODE_OF_CONDUCT.md) for details.

## License

This project is licensed under the [MIT License](https://truelayer.mit-license.org/).
