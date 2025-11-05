# Payouts

Process payouts to send funds from your merchant account to beneficiaries. Payouts are perfect for marketplace disbursements, refunds, and payments to suppliers.

## Basic Payout Creation

### Payout to UK Account

```csharp
var request = new CreatePayoutRequest(
    merchantAccountId: "your-merchant-account-id",
    amountInMinor: 10000, // £100.00
    currency: Currencies.GBP,
    beneficiary: new Beneficiary.ExternalAccount(
        accountHolderName: "John Smith",
        reference: "Payment for services - Invoice #123",
        accountIdentifier: new AccountIdentifier.SortCodeAccountNumber("20-71-06", "44377677")
    )
);

var response = await _client.Payouts.CreatePayout(
    request,
    idempotencyKey: Guid.NewGuid().ToString()
);

if (response.IsSuccessful)
{
    var payoutId = response.Data!.Match(
        authRequired => authRequired.Id,
        created => created.Id
    );

    Console.WriteLine($"Payout created: {payoutId}");
}
```

### Payout to IBAN

For international or SEPA transfers:

```csharp
var request = new CreatePayoutRequest(
    merchantAccountId: merchantAccountId,
    amountInMinor: 50000, // £500.00
    currency: Currencies.GBP,
    beneficiary: new Beneficiary.ExternalAccount(
        accountHolderName: "ACME Corp",
        reference: "Invoice payment INV-2024-001",
        accountIdentifier: new AccountIdentifier.Iban("GB33BUKB20201555555555")
    )
);
```

## Complete Beneficiary Information

### With Address and Date of Birth

```csharp
var beneficiary = new Beneficiary.ExternalAccount(
    accountHolderName: "Jane Doe",
    reference: "Marketplace payout - Order #789",
    accountIdentifier: new AccountIdentifier.SortCodeAccountNumber("20-71-06", "12345678"),
    dateOfBirth: new DateTime(1985, 6, 15),
    address: new Address(
        city: "London",
        state: "England",
        zip: "EC1R 4RB",
        countryCode: "GB",
        addressLine1: "123 High Street",
        addressLine2: "Flat 2"
    )
);

var request = new CreatePayoutRequest(
    merchantAccountId: merchantAccountId,
    amountInMinor: 25000,
    currency: Currencies.GBP,
    beneficiary: beneficiary
);
```

## Adding Metadata

Store custom data with your payout:

```csharp
var request = new CreatePayoutRequest(
    merchantAccountId: merchantAccountId,
    amountInMinor: 10000,
    currency: Currencies.GBP,
    beneficiary: beneficiary,
    metadata: new Dictionary<string, string>
    {
        { "order_id", "ORD-2024-001" },
        { "seller_id", "SELLER-123" },
        { "marketplace_fee", "500" }
    }
);
```

## Payout Status

Payouts transition through various statuses as they are processed. Understanding these statuses helps you track payout progress and handle different scenarios appropriately.

For complete details, see the [TrueLayer Payout and Refund Status documentation](https://docs.truelayer.com/docs/payout-and-refund-statuses).

### Status Overview

| Status | Description | Terminal | Notes |
|--------|-------------|----------|-------|
| `authorization_required` | Payout created but requires additional authorization | No | Complete authorization flow |
| `pending` | Payout created but not yet authorized or sent to payment scheme | No | Waiting for authorization |
| `authorized` | Sent to payment scheme for execution | No | Processing by payment scheme |
| `executed` | Payout amount deducted from merchant account | Yes | Complete - sent to beneficiary* |
| `failed` | Payout did not complete, amount not deducted | Yes | Check `FailureReason` for details |

**Note:** `executed` means the amount has been deducted from your merchant account and sent to the payment scheme, but is not a guarantee that the beneficiary has received the funds.

### Common Failure Reasons

When a payout reaches `failed` status, check the `FailureReason` property for details:

| Failure Reason | Description |
|----------------|-------------|
| `blocked` | Blocked by regulatory requirements |
| `insufficient_funds` | Not enough money in merchant account |
| `invalid_iban` | Invalid beneficiary account identifier |
| `returned` | Rejected by beneficiary bank after execution |
| `scheme_error` | Issue with payment provider/scheme |
| `server_error` | TrueLayer processing error |
| `unknown` | Unspecified failure reason |

**Note:** Always handle unexpected failure reasons defensively, as new reasons may be added.

### Checking Payout Status

```csharp
var response = await _client.Payouts.GetPayout(payoutId);

if (response.IsSuccessful)
{
    response.Data.Match(
        authRequired =>
        {
            Console.WriteLine($"Status: {authRequired.Status}");
            Console.WriteLine("Action: Complete authorization flow");
            Console.WriteLine($"Authorization URL: {authRequired.AuthorizationFlow}");
        },
        pending =>
        {
            Console.WriteLine($"Status: {pending.Status}");
            Console.WriteLine("Action: Waiting for authorization or scheme processing");
        },
        authorized =>
        {
            Console.WriteLine($"Status: {authorized.Status}");
            Console.WriteLine("Action: Payment scheme is processing the payout");
        },
        executed =>
        {
            Console.WriteLine($"Status: {executed.Status}");
            Console.WriteLine("Terminal: Payout completed - funds deducted from merchant account");
        },
        failed =>
        {
            Console.WriteLine($"Status: {failed.Status}");
            Console.WriteLine($"Failure Reason: {failed.FailureReason}");
            Console.WriteLine("Terminal: Payout failed - no funds deducted");
        }
    );
}
```

### Handling Terminal Statuses

```csharp
public bool IsTerminalStatus(GetPayoutResponse payout)
{
    return payout.Match(
        authRequired => false,
        pending => false,
        authorized => false,
        executed => true,   // Terminal - payout complete
        failed => true      // Terminal - payout failed
    );
}

public async Task WaitForTerminalStatus(string payoutId, TimeSpan timeout)
{
    var startTime = DateTime.UtcNow;

    while (DateTime.UtcNow - startTime < timeout)
    {
        var response = await _client.Payouts.GetPayout(payoutId);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Failed to get payout status: {response.Problem?.Detail}");
        }

        if (IsTerminalStatus(response.Data))
        {
            return; // Payout reached terminal status
        }

        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    throw new TimeoutException("Payout did not reach terminal status within timeout");
}
```

### Handling Specific Failure Reasons

```csharp
public async Task<PayoutResult> HandlePayoutFailure(string payoutId)
{
    var response = await _client.Payouts.GetPayout(payoutId);

    if (!response.IsSuccessful)
    {
        return PayoutResult.Error("Failed to retrieve payout status");
    }

    return response.Data.Match<PayoutResult>(
        authRequired => PayoutResult.RequiresAuth(authRequired.AuthorizationFlow),
        pending => PayoutResult.Pending(),
        authorized => PayoutResult.Processing(),
        executed => PayoutResult.Success(),
        failed => failed.FailureReason switch
        {
            "insufficient_funds" => PayoutResult.Error(
                "Insufficient funds in merchant account. Please top up and retry."
            ),
            "invalid_iban" => PayoutResult.Error(
                "Invalid beneficiary account details. Please verify and create new payout."
            ),
            "blocked" => PayoutResult.Error(
                "Payout blocked by regulatory requirements. Contact support."
            ),
            "returned" => PayoutResult.Error(
                "Payout rejected by beneficiary bank after execution."
            ),
            "scheme_error" => PayoutResult.Error(
                "Payment scheme error. Retry may succeed."
            ),
            _ => PayoutResult.Error($"Payout failed: {failed.FailureReason}")
        }
    );
}
```

## Handling Authorization Required

Some payouts may require additional authorization:

```csharp
var response = await _client.Payouts.CreatePayout(request, idempotencyKey);

if (response.IsSuccessful)
{
    response.Data!.Match(
        authRequired =>
        {
            // Handle authorization flow
            Console.WriteLine($"Authorization required for payout: {authRequired.Id}");
            Console.WriteLine($"Authorization URL: {authRequired.AuthorizationFlow}");
            return authRequired.Id;
        },
        created =>
        {
            // Payout created successfully
            Console.WriteLine($"Payout created: {created.Id}");
            return created.Id;
        }
    );
}
```

## Idempotency

Prevent duplicate payouts using idempotency keys:

```csharp
public class PayoutTransaction
{
    public string TransactionId { get; set; }
    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();
    public string PayoutId { get; set; }
    public string BeneficiaryAccountNumber { get; set; }
    public int AmountInMinor { get; set; }
}

// Store idempotency key with transaction
var transaction = new PayoutTransaction
{
    TransactionId = "TXN-001",
    BeneficiaryAccountNumber = "12345678",
    AmountInMinor = 10000
};

// Safe to retry with same key
var response = await _client.Payouts.CreatePayout(
    request,
    idempotencyKey: transaction.IdempotencyKey
);

if (response.IsSuccessful)
{
    transaction.PayoutId = response.Data!.Match(
        authRequired => authRequired.Id,
        created => created.Id
    );

    await _repository.SaveAsync(transaction);
}
```

## Best Practices

### 1. Validate Beneficiary Details

```csharp
public class BeneficiaryValidator
{
    public bool ValidateSortCode(string sortCode)
    {
        // Validate format: XX-XX-XX or XXXXXX
        var cleaned = sortCode.Replace("-", "");
        return cleaned.Length == 6 && cleaned.All(char.IsDigit);
    }

    public bool ValidateAccountNumber(string accountNumber)
    {
        // UK account numbers are 8 digits
        return accountNumber.Length == 8 && accountNumber.All(char.IsDigit);
    }

    public bool ValidateIban(string iban)
    {
        // Basic IBAN validation
        var cleaned = iban.Replace(" ", "").ToUpper();
        return cleaned.Length >= 15 && cleaned.Length <= 34;
    }
}
```

### 2. Handle Errors Gracefully

```csharp
if (!response.IsSuccessful)
{
    _logger.LogError(
        "Payout creation failed. TraceId: {TraceId}, Status: {Status}, Detail: {Detail}",
        response.TraceId,
        response.StatusCode,
        response.Problem?.Detail
    );

    // Check specific error types
    if (response.StatusCode == HttpStatusCode.BadRequest)
    {
        // Validation errors
        foreach (var (field, errors) in response.Problem?.Errors ?? new Dictionary<string, string[]>())
        {
            _logger.LogWarning("Validation error on {Field}: {Errors}", field, string.Join(", ", errors));
        }
    }

    return null;
}
```

### 3. Store Payout Records

```csharp
public class PayoutRecord
{
    public string Id { get; set; }
    public string PayoutId { get; set; }
    public string IdempotencyKey { get; set; }
    public string MerchantAccountId { get; set; }
    public string BeneficiaryName { get; set; }
    public string BeneficiaryAccount { get; set; }
    public int AmountInMinor { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public string Reference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}
```

### 4. Reconciliation

Track payouts for accounting:

```csharp
public async Task<List<PayoutRecord>> GetPayoutsForReconciliation(DateTime date)
{
    var payouts = await _repository.GetPayoutsByDate(date);
    var reconciled = new List<PayoutRecord>();

    foreach (var payout in payouts)
    {
        var response = await _client.Payouts.GetPayout(payout.PayoutId);

        if (response.IsSuccessful)
        {
            payout.Status = response.Data.Match(
                authRequired => "authorization_required",
                pending => "pending",
                authorized => "authorized",
                executed => "executed",
                failed => "failed"
            );

            reconciled.Add(payout);
        }
    }

    return reconciled;
}
```

### 5. Rate Limiting

Implement batching for bulk payouts:

```csharp
public async Task<List<string>> ProcessBulkPayouts(List<PayoutRequest> payoutRequests)
{
    var payoutIds = new List<string>();
    var semaphore = new SemaphoreSlim(5); // Max 5 concurrent requests

    var tasks = payoutRequests.Select(async request =>
    {
        await semaphore.WaitAsync();
        try
        {
            var response = await _client.Payouts.CreatePayout(
                request.ToCreatePayoutRequest(),
                idempotencyKey: request.IdempotencyKey
            );

            if (response.IsSuccessful)
            {
                return response.Data!.Match(
                    authRequired => authRequired.Id,
                    created => created.Id
                );
            }

            return null;
        }
        finally
        {
            semaphore.Release();
        }
    });

    var results = await Task.WhenAll(tasks);
    return results.Where(id => id != null).ToList();
}
```

## Common Scenarios

### Marketplace Seller Payout

```csharp
public async Task<string> PayoutToSeller(Order order, Seller seller)
{
    var beneficiary = new Beneficiary.ExternalAccount(
        accountHolderName: seller.AccountName,
        reference: $"Order {order.Id} - {order.CustomerName}",
        accountIdentifier: new AccountIdentifier.SortCodeAccountNumber(
            seller.SortCode,
            seller.AccountNumber
        )
    );

    var request = new CreatePayoutRequest(
        merchantAccountId: _config.MerchantAccountId,
        amountInMinor: order.SellerAmount,
        currency: order.Currency,
        beneficiary: beneficiary,
        metadata: new Dictionary<string, string>
        {
            { "order_id", order.Id },
            { "seller_id", seller.Id },
            { "commission", order.Commission.ToString() }
        }
    );

    var response = await _client.Payouts.CreatePayout(
        request,
        idempotencyKey: order.PayoutIdempotencyKey
    );

    if (!response.IsSuccessful)
    {
        throw new PayoutException($"Failed to create payout: {response.Problem?.Detail}");
    }

    var payoutId = response.Data!.Match(
        authRequired => authRequired.Id,
        created => created.Id
    );

    // Record payout
    await _payoutRepository.CreateAsync(new PayoutRecord
    {
        PayoutId = payoutId,
        OrderId = order.Id,
        SellerId = seller.Id,
        AmountInMinor = order.SellerAmount,
        Status = "pending",
        CreatedAt = DateTime.UtcNow
    });

    return payoutId;
}
```

### Refund Processing

```csharp
public async Task<string> IssueRefund(Payment payment, int refundAmountInMinor)
{
    var beneficiary = new Beneficiary.ExternalAccount(
        accountHolderName: payment.CustomerName,
        reference: $"Refund for payment {payment.Id}",
        accountIdentifier: payment.SourceAccountIdentifier
    );

    var request = new CreatePayoutRequest(
        merchantAccountId: payment.MerchantAccountId,
        amountInMinor: refundAmountInMinor,
        currency: payment.Currency,
        beneficiary: beneficiary,
        metadata: new Dictionary<string, string>
        {
            { "payment_id", payment.Id },
            { "refund_reason", "customer_request" },
            { "original_amount", payment.AmountInMinor.ToString() }
        }
    );

    var response = await _client.Payouts.CreatePayout(
        request,
        idempotencyKey: $"refund-{payment.Id}-{Guid.NewGuid()}"
    );

    return response.Data!.Match(
        authRequired => authRequired.Id,
        created => created.Id
    );
}
```

### Scheduled Payouts

```csharp
public class ScheduledPayoutService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var pendingPayouts = await _repository.GetPendingPayouts();

            foreach (var payout in pendingPayouts.Where(p => p.ScheduledFor <= DateTime.UtcNow))
            {
                try
                {
                    await ProcessPayout(payout);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process scheduled payout {PayoutId}", payout.Id);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task ProcessPayout(ScheduledPayout payout)
    {
        var request = new CreatePayoutRequest(
            merchantAccountId: payout.MerchantAccountId,
            amountInMinor: payout.AmountInMinor,
            currency: payout.Currency,
            beneficiary: payout.ToBeneficiary()
        );

        var response = await _client.Payouts.CreatePayout(
            request,
            idempotencyKey: payout.IdempotencyKey
        );

        if (response.IsSuccessful)
        {
            payout.PayoutId = response.Data!.Match(
                authRequired => authRequired.Id,
                created => created.Id
            );
            payout.Status = "created";
            await _repository.UpdateAsync(payout);
        }
    }
}
```

## Currency Support

Payouts support multiple currencies:

```csharp
// GBP Payout
var gbpRequest = new CreatePayoutRequest(
    merchantAccountId: gbpMerchantAccountId,
    amountInMinor: 10000,
    currency: Currencies.GBP,
    beneficiary: gbpBeneficiary
);

// EUR Payout
var eurRequest = new CreatePayoutRequest(
    merchantAccountId: eurMerchantAccountId,
    amountInMinor: 10000,
    currency: Currencies.EUR,
    beneficiary: eurBeneficiary
);
```

## Security Considerations

### 1. Verify Beneficiary Before Payout

```csharp
public async Task<bool> VerifyBeneficiary(string accountNumber, string sortCode)
{
    // Implement verification logic
    // - Check against whitelist
    // - Verify account exists
    // - Validate beneficiary identity
    return true;
}
```

### 2. Audit Trail

```csharp
public class PayoutAuditLog
{
    public string PayoutId { get; set; }
    public string InitiatedBy { get; set; }
    public DateTime InitiatedAt { get; set; }
    public string BeneficiaryAccount { get; set; }
    public int AmountInMinor { get; set; }
    public string Status { get; set; }
    public string IpAddress { get; set; }
}
```

### 3. Two-Factor Authentication

Consider implementing 2FA for large payouts:

```csharp
public async Task<string> CreateHighValuePayout(CreatePayoutRequest request, string twoFactorCode)
{
    // Verify 2FA code
    if (!await _twoFactorService.VerifyCode(twoFactorCode))
    {
        throw new UnauthorizedException("Invalid 2FA code");
    }

    // Proceed with payout
    var response = await _client.Payouts.CreatePayout(request, Guid.NewGuid().ToString());

    return response.Data!.Match(
        authRequired => authRequired.Id,
        created => created.Id
    );
}
```

## See Also

- [Merchant Accounts](merchant-accounts.md) - Manage your merchant accounts
- [Error Handling](error-handling.md) - Handle payout errors
- [API Reference](xref:TrueLayer.Payouts.IPayoutsApi) - Complete API documentation
