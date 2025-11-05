# Mandates

Mandates allow you to set up recurring payment authority from a user's bank account. Once authorized, you can create payments against the mandate without requiring the user to re-authenticate each time.

## Types of Mandates

TrueLayer supports two types of mandates:

- **Sweeping** - For variable recurring payments (VRP)
- **Commercial** - For commercial transactions

## Creating a Mandate

### Basic Mandate Creation

```csharp
public async Task<string> CreateMandate()
{
    var mandateRequest = new CreateMandateRequest(
        mandateType: MandateType.Sweeping,
        providerSelection: new CreateProviderSelection.UserSelected(),
        beneficiary: new Beneficiary.MerchantAccount("your-merchant-account-id"),
        user: new MandateUserRequest(
            name: "John Doe",
            email: "john.doe@example.com"
        ),
        constraints: new Constraints(
            maxAmountPerPayment: new AmountConstraint
            {
                MinimumAmount = 1,
                MaximumAmount = 10000
            },
            periodicLimits: new PeriodicLimits
            {
                Month = new PeriodicLimit
                {
                    MaximumAmount = 50000,
                    PeriodAlignment = PeriodAlignment.Consent
                }
            }
        )
    );

    var response = await _client.Mandates.CreateMandate(
        mandateRequest,
        idempotencyKey: Guid.NewGuid().ToString()
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"Mandate creation failed: {response.Problem}");
    }

    // Redirect user to authorization page
    var redirectUrl = _client.Mandates.CreateHostedPaymentPageLink(
        response.Data.Id,
        response.Data.ResourceToken,
        new Uri("https://yourdomain.com/mandate/callback")
    );

    return redirectUrl;
}
```

## Retrieving a Mandate

```csharp
public async Task<MandateDetail> GetMandate(string mandateId)
{
    var response = await _client.Mandates.GetMandate(
        mandateId,
        MandateType.Sweeping
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"Failed to retrieve mandate: {response.Problem}");
    }

    return response.Data.Match(
        authRequired => (MandateDetail)authRequired,
        authorizing => authorizing,
        authorized => authorized,
        failed => throw new Exception($"Mandate failed: {failed.FailureReason}"),
        revoked => throw new Exception("Mandate has been revoked")
    );
}
```

## Listing User Mandates

```csharp
public async Task<List<MandateDetail>> ListUserMandates(string userId)
{
    var query = new ListMandatesQuery
    {
        UserId = userId,
        Limit = 20
    };

    var response = await _client.Mandates.ListMandates(
        query,
        MandateType.Sweeping
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"Failed to list mandates: {response.Problem}");
    }

    return response.Data.Items.ToList();
}
```

## Custom Authorization Flow

For more control over the authorization process:

```csharp
public async Task<string> StartMandateAuthFlow(string mandateId)
{
    var request = new StartAuthorizationFlowRequest
    {
        ProviderSelection = new ProviderSelection.UserSelected
        {
            ProviderId = "mock-payments-gb-redirect",
            SchemeId = "faster_payments_service"
        },
        Redirect = new Redirect
        {
            ReturnUri = new Uri("https://yourdomain.com/callback"),
            DirectReturnUri = new Uri("https://yourdomain.com/direct-callback")
        }
    };

    var response = await _client.Mandates.StartAuthorizationFlow(
        mandateId,
        request,
        idempotencyKey: Guid.NewGuid().ToString(),
        MandateType.Sweeping
    );

    return response.Data.Match(
        authorizing => authorizing.AuthorizationFlow.Actions.Next.Uri.ToString(),
        failed => throw new Exception($"Authorization failed: {failed.FailureReason}")
    );
}
```

## Confirmation of Funds

Check if sufficient funds are available before creating a payment:

```csharp
public async Task<bool> CheckFunds(string mandateId, int amountInMinor)
{
    var response = await _client.Mandates.GetConfirmationOfFunds(
        mandateId,
        amountInMinor,
        "GBP",
        MandateType.Sweeping
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"CoF check failed: {response.Problem}");
    }

    return response.Data.Confirmed;
}
```

## Getting Mandate Constraints

Retrieve the constraints applied to a mandate:

```csharp
public async Task<Constraints> GetConstraints(string mandateId)
{
    var response = await _client.Mandates.GetMandateConstraints(
        mandateId,
        MandateType.Sweeping
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"Failed to get constraints: {response.Problem}");
    }

    return response.Data.Constraints;
}
```

## Revoking a Mandate

Cancel a mandate when no longer needed:

```csharp
public async Task RevokeMandate(string mandateId)
{
    var response = await _client.Mandates.RevokeMandate(
        mandateId,
        idempotencyKey: Guid.NewGuid().ToString(),
        MandateType.Sweeping
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"Failed to revoke mandate: {response.Problem}");
    }
}
```

## Mandate States

A mandate can be in one of the following states:

| State | Description |
|-------|-------------|
| `AuthorizationRequired` | User needs to authorize the mandate |
| `Authorizing` | Authorization is in progress |
| `Authorized` | Mandate is active and can be used for payments |
| `Failed` | Mandate authorization failed |
| `Revoked` | Mandate has been cancelled |

## Mandate Constraints

Configure limits on payments made under the mandate:

### Amount Constraints

```csharp
var constraints = new Constraints(
    maxAmountPerPayment: new AmountConstraint
    {
        MinimumAmount = 100,      // Min 1.00 GBP
        MaximumAmount = 100000    // Max 1,000.00 GBP
    }
);
```

### Periodic Limits

```csharp
var constraints = new Constraints(
    periodicLimits: new PeriodicLimits
    {
        Day = new PeriodicLimit
        {
            MaximumAmount = 10000,
            PeriodAlignment = PeriodAlignment.Consent
        },
        Month = new PeriodicLimit
        {
            MaximumAmount = 50000,
            PeriodAlignment = PeriodAlignment.Calendar
        }
    }
);
```

### Valid Payment Types

Restrict which payment types can be used:

```csharp
var constraints = new Constraints(
    validPaymentTypes: new[] { "domestic_payment", "international_payment" }
);
```

## Best Practices

### 1. Store Mandate IDs Securely

Store mandate IDs with user records to enable future payments:

```csharp
public class User
{
    public string Id { get; set; }
    public string MandateId { get; set; }
    public DateTime MandateAuthorizedAt { get; set; }
}
```

### 2. Check Mandate Status Before Payment

Always verify mandate is in `Authorized` state:

```csharp
var mandate = await GetMandate(mandateId);
if (mandate is not MandateDetail.AuthorizedMandateDetail)
{
    throw new InvalidOperationException("Mandate is not authorized");
}
```

### 3. Use Confirmation of Funds

For large payments, check funds are available:

```csharp
var fundsAvailable = await CheckFunds(mandateId, paymentAmount);
if (!fundsAvailable)
{
    // Handle insufficient funds
}
```

### 4. Handle Revocations

Users can revoke mandates from their bank. Handle this gracefully:

```csharp
try
{
    var mandate = await GetMandate(mandateId);
}
catch (Exception ex) when (ex.Message.Contains("revoked"))
{
    // Remove mandate from user record
    // Notify user to set up new mandate
}
```

## Next Steps

- [Payments Guide](payments.md)
- [Handle Errors](error-handling.md)
- [API Reference](xref:TrueLayer.Mandates.IMandatesApi)
