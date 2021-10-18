# TrueLayer .NET

The **TrueLayer .NET library** enables .NET developers to easily work with [TrueLayer APIs](https://docs.truelayer.com/).

## Quickstart

Install the TrueLayer NuGet package:

```
dotnet add package TrueLayer
```

Add your `ClientId` and `ClientSecret` to `appsettings.json`. You can obtain them by signing up at [TrueLayer's console](https://console.truelayer.com/?auto=signup).


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

Register the TrueLayer client in `Startup.cs`:

```c#
public IConfiguration Configuration { get; }

public void ConfigureServices(IServiceCollection services)
{
    services.AddTrueLayer(Configuration);

    // Or if using APIs that require request signing e.g. payments
    services.AddTrueLayer(configuration, options =>
    {
        if (options.Payments?.SigningKey != null)
        {
            // For demo purposes only. Private keys should always be stored securely
            options.Payments.SigningKey.Certificate = File.ReadAllText("ec512-private-key.pem");
        }
    });
}
```

Inject `ITrueLayerClient` into your classes:

```c#
class MyService
{
    private readonly ITrueLayerClient _client_;

    public MyService(ITrueLayerClient client)
    {
        _client = client;
    }

    public async Task<ActionResult> MakePayment()
    {
        var paymentRequest = new CreatePaymentRequest(
            100,
            Currencies.GBP,
            PaymentMethod.BankTransfer(providerFilter: new ProviderFilter
            {
                ProviderIds = new[] { "mock-payments-gb-redirect" }
            }),
            new ExternalAccountBeneficiary(
                "TrueLayer",
                "truelayer-dotnet",
                new SortCodeAccountNumberSchemeIdentifier("567890", "12345678")
            )
        );

        var response = await _client.Payments.CreatePayment(
            paymentRequest, 
            idempotencyKey: Guid.NewGuid().ToString()
        );

        string hostedPaymentPageUrl = response.Data.Match(
            authRequired => _client.CreateHostedPaymentPageLink(
                authRequired.Id, authRequired.ResourceToken, new Uri("https://redirect.yourdomain.com")
            )
        );

        // Redirect to the TrueLayer Hosted Payment Page
        return Redirect(hostedPaymentPageUrl);
    }
}
```

All API operations return an `ApiResponse<TData>` where `TData` contains the result of the API call. Get more details on [TrueLayer's API documentation](https://docs.truelayer.com/).


### Pre-release Packages

Pre-release packages can be downloaded from [GitHub Packages](https://github.com/truelayer?tab=packages&repo_name=truelayer-dotnet).

```
dotnet add package TrueLayer --prerelease --source https://nuget.pkg.github.com/truelayer/index.json
```

[More information](https://docs.github.com/en/packages/guides/configuring-dotnet-cli-for-use-with-github-packages) on using GitHub packages with .NET.

## Building locally 

This project uses [Cake](https://cakebuild.net/) to build, test and publish packages. 

Run `build.sh` (Mac/Linux) or `build.ps1` (Windows) To build and test the project. 

This will output NuGet packages and coverage reports in the `artifacts` directory.

## Contributing

To contribute to TrueLayer for .NET, fork the repository and raise a PR. If your change is substantial please [open an issue](https://github.com/benfoster/o9d-json/issues) first to discuss your objective.

## Docs

The JSON documentation is built using [DocFx](https://dotnet.github.io/docfx/). To build and serve the docs locally run:

```
./build.sh --target ServeDocs
```

This will serve the docs on http://localhost:8080.
