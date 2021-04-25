# truelayer-sdk-net
The **TrueLayer SDK for .NET** enables .NET developers to easily work with [TrueLayer.com APIs](https://docs.truelayer.com/). It supports .NET Core and .NET 5. It is (heavily) inspired to [Checkout's .NET Sdk](https://github.com/checkout/checkout-sdk-net).

## Quickstart
Make sure you fill the `clientId` and `clientSecret` fields inside your `appsettings.json`. You can obtain them by signing up on [Truelayer's console](https://console.truelayer.com/?auto=signup).
```JavaScript
{
  "Truelayer": {
    "ClientId": "",
    "ClientSecret": "",
    "UseSandbox": ""
  }
}
```

Initialize a TruelayerApi instance to access the operations for each API:
```C#
var api = TruelayerApi.Create(config["clientId"], config["clientSecret"], useSandbox: true);

// Gather an access token which will be used for the payment request
var auth = await api.Auth.GetPaymentToken(new GetPaymentTokenRequest());
// Initiate a payment
var response = await api.Payments.SingleImmediatePayment(request);
var paymentId = response.results.First().simp_id;
var authUri = response.results.First().auth_uri;
```
All API operations return an ApiResponse<TResult> where TResult contains the result of the API call. Get more details on [Truelayer's API documentation](https://docs.truelayer.com/).

## .NET Core / .NET 5 Applications
You can leverage the .NET built-in Dependency Injection system to add the Truelayer SDK inside your application.

Reference directly the `TrueLayerSdk.Extensions.Microsoft` project or install the nuget when available, and register the SDK with the built-in DI container in `Startup.cs`:

```C#
public IConfiguration Configuration { get; }

public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddTruelayerSdk(Configuration);
    // OR
    var tlConfiguration = new TruelayerConfiguration("clientId", "clientSecret", useSandbox: true);
    services.AddTruelayerSdk(tlConfiguration);
    //...
}
```

# Pre-alpha checklist

## APIs
- [ ] Auth
  - [x] Payment token
- [ ] Payments v1
  - [ ] Providers list
  - [x] Single immediate payment
  - [ ] Webhooks
  - [x] Payment status
  - [ ] Payment status history
  - [ ] Standing order
  - [ ] Standing order status
- [ ] Payments v2
  - [ ] Providers list
  - [x] Single immediate payment initiation
    - [x] Redirect
    - [ ] Embedded
  - [ ] Webhooks
  - [ ] Payment status
- [ ] Data

## Various arguments

### Not yet ported from Checkout SDK
- [ ] Idempotency key
- [ ] Product version
- [ ] Json serializer options
- [ ] Logging
- [ ] Tl-Request-Id
- [ ] Unprocessable

### Known todos
- [ ] Tests
- [ ] Ben's `Guard` [nuget](https://github.com/benfoster/o9d-guard)
