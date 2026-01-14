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

## Mandate Status

Mandates transition through various statuses during their lifecycle. Understanding these statuses helps you track mandate progress and handle different scenarios appropriately.

For complete details, see the [TrueLayer Mandate Status documentation](https://docs.truelayer.com/docs/mandate-statuses).

### Status Overview

| Status | Description | Terminal | Notes |
|--------|-------------|----------|-------|
| `authorization_required` | Mandate created but no further action taken | No | User needs to authorize the mandate |
| `authorizing` | User has started but not completed authorization journey | No | Wait for webhook notification |
| `authorized` | User has successfully completed authorization flow | No | Mandate is active and can be used for payments |
| `revoked` | Mandate has been cancelled | Yes | Can be revoked by client or user's bank |
| `failed` | Mandate could not be authorized | Yes | Check `FailureReason` for details |

### Common Failure Reasons

When a mandate reaches `failed` status, check the `FailureReason` property for details:

| Failure Reason | Description |
|----------------|-------------|
| `authorization_failed` | User failed to complete authorization |
| `provider_error` | Error with the provider/bank |
| `provider_rejected` | Provider rejected the mandate |
| `internal_server_error` | TrueLayer processing error |
| `invalid_sort_code` | Invalid bank account sort code |
| `invalid_request` | Request validation failed |
| `expired` | Mandate authorization expired |
| `unknown_error` | Unspecified failure reason |

**Note:** Always handle unexpected failure reasons defensively, as new reasons may be added.

### Checking Mandate Status

```csharp
var response = await _client.Mandates.GetMandate(mandateId, MandateType.Sweeping);

if (response.IsSuccessful)
{
    response.Data.Match(
        authRequired =>
        {
            Console.WriteLine($"Status: {authRequired.Status}");
            Console.WriteLine("Action: User needs to authorize the mandate");
        },
        authorizing =>
        {
            Console.WriteLine($"Status: {authorizing.Status}");
            Console.WriteLine("Action: User is authorizing, wait for completion");
        },
        authorized =>
        {
            Console.WriteLine($"Status: {authorized.Status}");
            Console.WriteLine("Action: Mandate is active and can be used");
        },
        failed =>
        {
            Console.WriteLine($"Status: {failed.Status}");
            Console.WriteLine($"Failure Reason: {failed.FailureReason}");
            Console.WriteLine($"Failed At: {failed.FailedAt}");
            Console.WriteLine($"Failure Stage: {failed.FailureStage}");
            Console.WriteLine("Terminal: Mandate failed");
        },
        revoked =>
        {
            Console.WriteLine($"Status: {revoked.Status}");
            Console.WriteLine($"Revoked At: {revoked.RevokedAt}");
            Console.WriteLine($"Revoked By: {revoked.RevokedBy}");
            Console.WriteLine("Terminal: Mandate has been cancelled");
        }
    );
}
```

### Handling Terminal Statuses

```csharp
public bool IsTerminalStatus(GetMandateResponse mandate)
{
    return mandate.Match(
        authRequired => false,
        authorizing => false,
        authorized => false,
        failed => true,    // Terminal - mandate failed
        revoked => true    // Terminal - mandate cancelled
    );
}

public async Task WaitForAuthorization(string mandateId, TimeSpan timeout)
{
    var startTime = DateTime.UtcNow;

    while (DateTime.UtcNow - startTime < timeout)
    {
        var response = await _client.Mandates.GetMandate(mandateId, MandateType.Sweeping);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Failed to get mandate status: {response.Problem?.Detail}");
        }

        var isComplete = response.Data.Match(
            authRequired => false,
            authorizing => false,
            authorized => true,  // Success
            failed => throw new Exception($"Mandate failed: {failed.FailureReason}"),
            revoked => throw new Exception("Mandate was revoked")
        );

        if (isComplete)
        {
            return; // Mandate authorized successfully
        }

        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    throw new TimeoutException("Mandate did not complete authorization within timeout");
}
```

### Handling Specific Failure Reasons

```csharp
public async Task<MandateResult> HandleMandateFailure(string mandateId)
{
    var response = await _client.Mandates.GetMandate(mandateId, MandateType.Sweeping);

    if (!response.IsSuccessful)
    {
        return MandateResult.Error("Failed to retrieve mandate status");
    }

    return response.Data.Match<MandateResult>(
        authRequired => MandateResult.RequiresAuth(),
        authorizing => MandateResult.Authorizing(),
        authorized => MandateResult.Success(),
        failed => failed.FailureReason switch
        {
            "authorization_failed" => MandateResult.Error(
                "User failed to complete authorization. Please create a new mandate."
            ),
            "provider_rejected" => MandateResult.Error(
                "Provider rejected the mandate. The bank may not support this operation."
            ),
            "expired" => MandateResult.Error(
                "Mandate authorization expired. Please create a new mandate."
            ),
            "invalid_sort_code" => MandateResult.Error(
                "Invalid bank account details. Please verify and create new mandate."
            ),
            _ => MandateResult.Error($"Mandate failed: {failed.FailureReason}")
        },
        revoked => MandateResult.Error(
            $"Mandate was revoked by {revoked.RevokedBy} on {revoked.RevokedAt}"
        )
    );
}
```

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

Always verify mandate is in `authorized` status:

```csharp
var response = await _client.Mandates.GetMandate(mandateId, MandateType.Sweeping);

if (!response.IsSuccessful)
{
    throw new Exception("Failed to retrieve mandate");
}

var isAuthorized = response.Data.Match(
    authRequired => false,
    authorizing => false,
    authorized => true,
    failed => false,
    revoked => false
);

if (!isAuthorized)
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
