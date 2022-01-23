# TrueLayer.NET

[![NuGet](https://img.shields.io/nuget/v/TrueLayer.Client.svg)](https://www.nuget.org/packages/TrueLayer.Client)
[![NuGet](https://img.shields.io/nuget/vpre/TrueLayer.Client?label=Pre-release)](https://www.nuget.org/packages/TrueLayer.Client)
[![NuGet](https://img.shields.io/nuget/dt/TrueLayer.Client.svg)](https://www.nuget.org/packages/TrueLayer.Client)
[![License](https://img.shields.io/:license-mit-blue.svg)](https://truelayer.mit-license.org/)

## Installation

Using the [.NET Core command-line interface (CLI) tools](https://docs.microsoft.com/en-us/dotnet/core/tools/):

```sh
dotnet add package TrueLayer.Client
```

Using the [NuGet Command Line Interface (CLI)](https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference)

```sh
nuget install TrueLayer.Client
```

Using the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):

```powershell
Install-Package TrueLayer.Client
```

From within Visual Studio:

1. Open the Solution Explorer.
2. Right-click on a project within your solution.
3. Click on *Manage NuGet Packages...*
4. Click on the *Browse* tab and search for "TrueLayer".
5. Click on the `TrueLayer` package, select the appropriate version in the
   right-tab and click *Install*.

## Usage

### Prerequisites

First [sign up](https://console.truelayer.com/) for a developer account. Follow the instructions to set up a new application and obtain your Client ID and Secret.

Next, generate a signing key pair used to sign API requests.

To generate a private key, run:

```sh
docker run --rm -v ${PWD}:/out -w /out -it alpine/openssl ecparam -genkey -name secp521r1 -noout -out ec512-private-key.pem
```

To obtain the public key, run:

```sh
docker run --rm -v ${PWD}:/out -w /out -it alpine/openssl ec -in ec512-private-key.pem -pubout -out ec512-public-key.pem
```

Navigate to the Payments settings section of the TrueLayer console and upload your public key. Obtain the Key Id.

### Configure Settings

Add your Client ID, Secret and Signing Key ID to `appsettings.json` or any other supported [configuration provider](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration).


```json
{
  "TrueLayer": {
    "ClientId": "your id",
    "ClientSecret": "your secret",
    "UseSandbox": true,
    "Payments": {
      "SigningKey": {
        "KeyId": "85eeb2da-702c-4f4b-bf9a-e98af5fd47c3"
      }
    }
  }
}
```

### Initialize TrueLayer.NET

Register the TrueLayer client in `Startup.cs` or `Program.cs` (.NET 6.0):

```c#
public IConfiguration Configuration { get; }

public void ConfigureServices(IServiceCollection services)
{
    services.AddTrueLayer(configuration, options =>
    {
        if (options.Payments?.SigningKey != null)
        {
            // For demo purposes only. Private key should be stored securely
            options.Payments.SigningKey.PrivateKey = File.ReadAllText("ec512-private-key.pem");
        }
    });
}
```

### Make a payment

Inject `ITrueLayerClient` into your classes:

```c#
public class MyService
{
    private readonly ITrueLayerClient _client;

    public MyService(ITrueLayerClient client)
    {
        _client = client;
    }

    public async Task<ActionResult> MakePayment()
    {
        var paymentRequest = new CreatePaymentRequest(
            amountInMinor: 100,
            currency: Currencies.GBP,
            paymentMethod: new PaymentMethod.BankTransfer
            {
                StatementReference = "Your ref"
            },
            beneficiary: new Beneficiary.ExternalAccount(
                "TrueLayer",
                "truelayer-dotnet",
                new SchemeIdentifier.SortCodeAccountNumber("567890", "12345678")
            ),
            user: PaymentUserRequest.New("Jane Doe", "jane.doe@example.com", "0123456789")
        );

        var apiResponse = await _client.Payments.CreatePayment(
            paymentRequest,
            idempotencyKey: Guid.NewGuid().ToString()
        );

        if (!apiResponse.IsSuccessful)
        {
            return HandleFailure(
                apiResponse.StatusCode,
                // Includes details of any errors
                apiResponse.Problem
            )
        }

        // Pass the PaymentToken to the TrueLayer Web or Mobile SDK

        // or, redirect to the TrueLayer Hosted Payment Page
        string hostedPaymentPageUrl = _client.Payments.CreateHostedPaymentPageLink(
            apiResponse.Data!.Id,
            apiResponse.Data!.PaymentToken,
            new Uri("https://redirect.yourdomain.com"));

        return Redirect(hostedPaymentPageUrl);
    }
}
```
