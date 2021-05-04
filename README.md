# truelayer-sdk-net
The **TrueLayer SDK for .NET** enables .NET developers to easily work with [TrueLayer.com APIs](https://docs.truelayer.com/). It supports .NET Core and .NET 5. It is (heavily) inspired to [Checkout's .NET Sdk](https://github.com/checkout/checkout-sdk-net).

## Quickstart

Install the TrueLayer SDK NuGet package:

```
dotnet add package TrueLayerSdk
```

Add your `ClientId` and `ClientSecret` to `appsettings.json`. You can obtain them by signing up at [TrueLayer's console](https://console.truelayer.com/?auto=signup).


```json
{
  "TrueLayer": {
    "ClientId": "your id",
    "ClientSecret": "your secret",
    "UseSandbox": true
  }
}
```

Register the TrueLayer SDK in `Startup.cs`:

```c#
public IConfiguration Configuration { get; }

public void ConfigureServices(IServiceCollection services)
{
    services.AddTrueLayerSdk(Configuration);
}
```

Inject `ITrueLayerApi` into your classes:

```c#
class MyService
{
    private readonly ITrueLayerApi _api;

    public MyService(ITrueLayerApi api)
    {
        _api = api;
    }

    public async Task MakePayment()
    {
        var request = new SingleImmediatePaymentRequest();
        var response = await _api.Payments.SingleImmediatePayment(request);
    }
}
```

All API operations return an `ApiResponse<TResult>` where `TResult` contains the result of the API call. Get more details on [TrueLayer's API documentation](https://docs.truelayer.com/).

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
