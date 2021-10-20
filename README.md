# TrueLayer.NET

[![NuGet](https://img.shields.io/nuget/v/TrueLayer.svg)](https://www.nuget.org/packages/TrueLayer) 
[![NuGet](https://img.shields.io/nuget/dt/TrueLayer.svg)](https://www.nuget.org/packages/TrueLayer)
[![License](https://img.shields.io/:license-mit-blue.svg)](https://truelayer.mit-license.org/)

![Build](https://github.com/truelayer/truelayer-dotnet/workflows/Build/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/truelayer/truelayer-dotnet/badge.svg?branch=main)](https://coveralls.io/github/truelayer/truelayer-dotnet?branch=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=truelayer_truelayer-dotnet&metric=alert_status)](https://sonarcloud.io/dashboard?id=truelayer_truelayer-dotnet)


The official [TrueLayer](https://truelayer.com) .NET library provides convenient access to TrueLayer APIs from applications built with .NET. 

The library currently supports .NET Standard 2.1, .NET 5.0 and .NET 6.0.

## Installation

Using the [.NET Core command-line interface (CLI) tools](https://docs.microsoft.com/en-us/dotnet/core/tools/):

```sh
dotnet add package TrueLayer
```

Using the [NuGet Command Line Interface (CLI)](https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference)

```sh
nuget install TrueLayer
```

Using the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):

```powershell
Install-Package TrueLayer
```

From within Visual Studio:

1. Open the Solution Explorer.
2. Right-click on a project within your solution.
3. Click on *Manage NuGet Packages...*
4. Click on the *Browse* tab and search for "TrueLayer".
5. Click on the `TrueLayer` package, select the appropriate version in the
   right-tab and click *Install*.

### Pre-release Packages

Pre-release packages can be downloaded from [GitHub Packages](https://github.com/truelayer?tab=packages&repo_name=truelayer-dotnet).

```
dotnet add package TrueLayer --prerelease --source https://nuget.pkg.github.com/truelayer/index.json
```

[More information](https://docs.github.com/en/packages/guides/configuring-dotnet-cli-for-use-with-github-packages) on using GitHub packages with .NET.

## Documentation

For a comprehensive list of examples, check out the [API documentation](https://docs.truelayer.com).

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
            options.Payments.SigningKey.Certificate = File.ReadAllText("ec512-private-key.pem");
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
            100,
            Currencies.GBP,
            new PaymentMethod.BankTransfer
            {
                StatementReference = "Your ref"
            },
            new Beneficiary.ExternalAccount(
                "TrueLayer",
                "truelayer-dotnet",
                new SchemeIdentifier.SortCodeAccountNumber("567890", "12345678")
            )
        );

        var apiResponse = await _client.Payments.CreatePayment(
            paymentRequest, 
            idempotencyKey: Guid.NewGuid().ToString()
        );

        // Pass the ResourceToken to the TrueLayer Web or Mobile SDK

        // or, redirect to the TrueLayer Hosted Payment Page

        string hostedPaymentPageUrl = apiResponse.Data.Match(
            authRequired => _client.CreateHostedPaymentPageLink(
                authRequired.Id, 
                authRequired.ResourceToken, 
                new Uri("https://redirect.yourdomain.com")
            )
        );

        return Redirect(hostedPaymentPageUrl);
    }
}
```

All API operations return an `ApiResponse<TData>` where `TData` contains the result of the API call. Get more details on [TrueLayer's API documentation](https://docs.truelayer.com/).

For more examples see the [API documentation](https://docs.truelayer.com). Advanced customization options and documentation for contributors can be found in the [Wiki](https://github.com/TrueLayer/truelayer-sdk-net/wiki).

## Building locally 

This project uses [Cake](https://cakebuild.net/) to build, test and publish packages. 

Run `build.sh` (Mac/Linux) or `build.ps1` (Windows) To build and test the project. 

This will output NuGet packages and coverage reports in the `artifacts` directory.

## Library Documentation

The library documentation is built using [DocFx](https://dotnet.github.io/docfx/). To build and serve the docs locally run:

```
./build.sh --target ServeDocs
```

This will serve the docs on http://localhost:8080.

## Contributing

Contributions are always welcome!

See [contributing](contributing.md) for ways to get started.

Please adhere to this project's [code of conduct](CODE_OF_CONDUCT.md).

  
## License

[MIT](LICENSE)
