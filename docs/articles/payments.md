# Payments

Create and manage payments using the TrueLayer Payments API. Payments allow you to initiate bank transfers from your users' accounts.

> **See also**: The [MVC Example](https://github.com/TrueLayer/truelayer-dotnet/tree/main/examples/MvcExample) demonstrates a complete payment flow in [PaymentsController.cs](https://github.com/TrueLayer/truelayer-dotnet/blob/main/examples/MvcExample/Controllers/PaymentsController.cs).

## Basic Payment Creation

### User-Selected Provider

Let the user choose their bank:

```csharp
var request = new CreatePaymentRequest(
    amountInMinor: 10000, // £100.00
    currency: Currencies.GBP,
    paymentMethod: new CreatePaymentMethod.BankTransfer(
        new CreateProviderSelection.UserSelected(),
        new Beneficiary.ExternalAccount(
            "Merchant Name",
            "merchant-reference",
            new AccountIdentifier.SortCodeAccountNumber("567890", "12345678")
        )
    ),
    user: new PaymentUserRequest("John Doe", "john@example.com")
);

var response = await _client.Payments.CreatePayment(
    request,
    idempotencyKey: Guid.NewGuid().ToString()
);

if (response.IsSuccessful)
{
    // Redirect user to authorization
    var redirectUrl = response.Data!.Match(
        authRequired => authRequired.HostedPage!.Uri.ToString(),
        authorized => $"Already authorized: {authorized.Id}",
        failed => throw new Exception($"Payment failed: {failed.FailureReason}"),
        authorizing => $"Authorizing: {authorizing.Id}"
    );
}
```

### Preselected Provider

Direct users to a specific bank:

```csharp
var request = new CreatePaymentRequest(
    amountInMinor: 5000,
    currency: Currencies.GBP,
    paymentMethod: new CreatePaymentMethod.BankTransfer(
        new CreateProviderSelection.Preselected(
            providerId: "mock-payments-gb-redirect",
            schemeSelection: new SchemeSelection.Preselected
            {
                SchemeId = "faster_payments_service"
            }
        ),
        new Beneficiary.ExternalAccount(
            "TrueLayer",
            "payment-ref-123",
            new AccountIdentifier.SortCodeAccountNumber("567890", "12345678")
        )
    ),
    user: new PaymentUserRequest("Jane Doe", "jane@example.com")
);
```

## Provider Filtering

Filter available banks for the user:

```csharp
var request = new CreatePaymentRequest(
    amountInMinor: 10000,
    currency: Currencies.GBP,
    paymentMethod: new CreatePaymentMethod.BankTransfer(
        new CreateProviderSelection.UserSelected
        {
            Filter = new ProviderFilter
            {
                ProviderIds = new[]
                {
                    "lloyds-bank",
                    "hsbc",
                    "barclays"
                }
            }
        },
        beneficiary
    ),
    user: userRequest
);
```

## Beneficiary Types

### External Account

Pay to an external bank account using different account identifier types:

#### Sort Code & Account Number

```csharp
var beneficiary = new Beneficiary.ExternalAccount(
    accountHolderName: "ACME Ltd",
    reference: "INV-2024-001",
    accountIdentifier: new AccountIdentifier.SortCodeAccountNumber("207106", "44377677")
);
```

#### IBAN

```csharp
var beneficiary = new Beneficiary.ExternalAccount(
    accountHolderName: "John Smith",
    reference: "Payment for services",
    accountIdentifier: new AccountIdentifier.Iban("GB33BUKB20201555555555")
);
```

### Merchant Account

Pay into your own merchant account:

```csharp
var beneficiary = new Beneficiary.MerchantAccount(
    merchantAccountId: "your-merchant-account-id",
    accountHolderName: "Your Business Ltd"
);
```

## User Information

### Basic User Details

```csharp
var user = new PaymentUserRequest(
    name: "John Doe",
    email: "john@example.com"
);
```

### Complete User Details

```csharp
var user = new PaymentUserRequest(
    name: "John Doe",
    email: "john@example.com",
    phone: "+447700900000",
    dateOfBirth: new DateTime(1990, 1, 1),
    address: new Address(
        city: "London",
        state: "England",
        zip: "EC1R 4RB",
        countryCode: "GB",
        addressLine1: "1 Hardwick St",
        addressLine2: "Clerkenwell"
    )
);
```

## Hosted Payment Page

### Recommended Approach

The recommended way to use TrueLayer's Hosted Payment Page is to include `HostedPageRequest` when creating the payment. This ensures the HPP URL is generated with the payment and returned in the response.

> **See also**: The [MVC Example](https://github.com/TrueLayer/truelayer-dotnet/tree/main/examples/MvcExample) uses `HostedPageRequest` in [PaymentsController.cs](https://github.com/TrueLayer/truelayer-dotnet/blob/main/examples/MvcExample/Controllers/PaymentsController.cs#L51-L69).

```csharp
var hostedPage = new HostedPageRequest(
    returnUri: new Uri("https://yourdomain.com/payment/callback"),
    countryCode: "GB",
    languageCode: "en"
);

var request = new CreatePaymentRequest(
    amountInMinor: 10000,
    currency: Currencies.GBP,
    paymentMethod: paymentMethod,
    user: user,
    hostedPage: hostedPage
);

var response = await _client.Payments.CreatePayment(
    request,
    idempotencyKey: Guid.NewGuid().ToString()
);

if (response.IsSuccessful)
{
    // HPP URL is included in the response when HostedPageRequest is used
    var hppUrl = response.Data!.Match(
        authRequired => authRequired.HostedPage!.Uri.ToString(),
        authorized => throw new Exception("Payment already authorized"),
        failed => throw new Exception($"Payment failed: {failed.FailureReason}"),
        authorizing => throw new Exception("Payment is authorizing")
    );

    return Redirect(hppUrl);
}
```

### Complete Example with Error Handling

```csharp
public async Task<IActionResult> CreatePaymentWithHostedPage(Order order)
{
    var request = new CreatePaymentRequest(
        amountInMinor: order.TotalInMinor,
        currency: Currencies.GBP,
        paymentMethod: new CreatePaymentMethod.BankTransfer(
            new CreateProviderSelection.UserSelected(),
            new Beneficiary.MerchantAccount(merchantAccountId)
        ),
        user: new PaymentUserRequest(order.CustomerName, order.CustomerEmail),
        hostedPage: new HostedPageRequest(
            returnUri: new Uri($"https://yourdomain.com/orders/{order.Id}/payment-return"),
            countryCode: "GB",
            languageCode: "en"
        )
    );

    var response = await _client.Payments.CreatePayment(
        request,
        idempotencyKey: order.PaymentIdempotencyKey
    );

    if (!response.IsSuccessful)
    {
        _logger.LogError(
            "Payment creation failed: {TraceId}",
            response.TraceId
        );
        return View("PaymentError");
    }

    // Store payment ID with order
    order.PaymentId = response.Data!.Id;
    await _orderRepository.UpdateAsync(order);

    // Redirect to TrueLayer's Hosted Payment Page
    var hppUrl = response.Data!.Match(
        authRequired => authRequired.HostedPage!.Uri.ToString(),
        _ => throw new Exception("Unexpected payment state")
    );

    return Redirect(hppUrl);
}
```

## Payment Status

Payments transition through various statuses as they progress. Understanding these statuses helps you track payment progress and handle different scenarios appropriately.

For complete details, see the [TrueLayer Payment Status documentation](https://docs.truelayer.com/docs/payment-statuses).

> **See also**: The [MVC Example](https://github.com/TrueLayer/truelayer-dotnet/tree/main/examples/MvcExample) demonstrates handling all payment statuses in [PaymentsController.cs](https://github.com/TrueLayer/truelayer-dotnet/blob/main/examples/MvcExample/Controllers/PaymentsController.cs#L114-L161).

### Status Overview

| Status | Description | Terminal | Notes |
|--------|-------------|----------|-------|
| `authorization_required` | Payment created successfully but no further action taken | No | Redirect user to Hosted Payment Page |
| `authorizing` | User started but hasn't completed authorization | No | Wait for webhook notification |
| `authorized` | User completed authorization; payment submitted to bank | No | No further user action needed |
| `executed` | Payment successfully submitted to bank | Yes* | Terminal for external account payments |
| `settled` | Payment settled into merchant account | Yes* | Terminal for merchant account payments |
| `failed` | Payment failed to progress | Yes | Check `FailureReason` for details |

**Terminal Status Notes:**
- For **merchant account** payments: `settled` or `failed` are terminal
- For **external account** payments: `executed` or `failed` are terminal

### Common Failure Reasons

When a payment reaches `failed` status, check the `FailureReason` property for details:

| Failure Reason | Description |
|----------------|-------------|
| `insufficient_funds` | User's account has insufficient funds |
| `canceled` | Payment was canceled by user or merchant |
| `expired` | Payment authorization expired |
| `provider_rejected` | Bank/provider rejected the payment |
| `invalid_account_details` | Beneficiary account details are invalid |

**Note:** Always handle unexpected failure reasons defensively, as new reasons may be added.

### Checking Payment Status

```csharp
var response = await _client.Payments.GetPayment(paymentId);

if (response.IsSuccessful)
{
    response.Data.Match(
        authRequired =>
        {
            Console.WriteLine($"Status: {authRequired.Status}");
            Console.WriteLine("Action: Redirect user to Hosted Payment Page");
        },
        authorizing =>
        {
            Console.WriteLine($"Status: {authorizing.Status}");
            Console.WriteLine("Action: Wait for authorization to complete");
        },
        authorized =>
        {
            Console.WriteLine($"Status: {authorized.Status}");
            Console.WriteLine("Action: Payment submitted to bank, waiting for execution");
        },
        executed =>
        {
            Console.WriteLine($"Status: {executed.Status}");
            Console.WriteLine("Terminal: Payment completed (external account)");
        },
        settled =>
        {
            Console.WriteLine($"Status: {settled.Status}");
            Console.WriteLine("Terminal: Payment completed (merchant account)");
        },
        failed =>
        {
            Console.WriteLine($"Status: {failed.Status}");
            Console.WriteLine($"Failure Reason: {failed.FailureReason}");
            Console.WriteLine("Terminal: Payment failed");
        },
        attemptFailed =>
        {
            Console.WriteLine($"Status: {attemptFailed.Status}");
            Console.WriteLine($"Failure Reason: {attemptFailed.FailureReason}");
            Console.WriteLine("Action: May retry automatically");
        }
    );
}
```

### Handling Terminal Statuses

```csharp
public bool IsTerminalStatus(GetPaymentResponse payment)
{
    return payment.Match(
        authRequired => false,
        authorizing => false,
        authorized => false,
        executed => true,  // Terminal for external accounts
        settled => true,   // Terminal for merchant accounts
        failed => true,
        attemptFailed => false  // May retry
    );
}

public async Task WaitForTerminalStatus(string paymentId, TimeSpan timeout)
{
    var startTime = DateTime.UtcNow;

    while (DateTime.UtcNow - startTime < timeout)
    {
        var response = await _client.Payments.GetPayment(paymentId);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Failed to get payment status: {response.Problem?.Detail}");
        }

        if (IsTerminalStatus(response.Data))
        {
            return; // Payment reached terminal status
        }

        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    throw new TimeoutException("Payment did not reach terminal status within timeout");
}
```

## Payment Refunds

### Full Refund

```csharp
var refundRequest = new CreatePaymentRefundRequest(
    amountInMinor: 10000, // Full amount
    reference: "Full refund for order #12345"
);

var response = await _client.Payments.CreatePaymentRefund(
    paymentId,
    idempotencyKey: Guid.NewGuid().ToString(),
    refundRequest
);

if (response.IsSuccessful)
{
    Console.WriteLine($"Refund ID: {response.Data.Id}");
}
```

### Partial Refund

```csharp
var refundRequest = new CreatePaymentRefundRequest(
    amountInMinor: 5000, // Partial amount
    reference: "Partial refund - damaged item"
);
```

### List All Refunds

```csharp
var response = await _client.Payments.ListPaymentRefunds(paymentId);

if (response.IsSuccessful)
{
    foreach (var refund in response.Data.Items)
    {
        refund.Match(
            pending => Console.WriteLine($"Pending: {pending.Id}"),
            authorized => Console.WriteLine($"Authorized: {authorized.Id}"),
            executed => Console.WriteLine($"Executed: {executed.Id}"),
            failed => Console.WriteLine($"Failed: {failed.FailureReason}")
        );
    }
}
```

### Get Refund Status

```csharp
var response = await _client.Payments.GetPaymentRefund(paymentId, refundId);

if (response.IsSuccessful)
{
    var status = response.Data.Match(
        pending => "Refund pending",
        authorized => "Refund authorized",
        executed => "Refund completed",
        failed => $"Refund failed: {failed.FailureReason}"
    );
}
```

## Cancelling Payments

Cancel a payment before it's executed:

```csharp
var response = await _client.Payments.CancelPayment(
    paymentId,
    idempotencyKey: Guid.NewGuid().ToString()
);

if (response.IsSuccessful)
{
    Console.WriteLine("Payment cancelled successfully");
}
else
{
    // Payment may have already been executed
    Console.WriteLine($"Cancellation failed: {response.Problem}");
}
```

## Custom Authorization Flow

For more control over the authorization process:

```csharp
var authRequest = new StartAuthorizationFlowRequest
{
    ProviderSelection = new ProviderSelection.UserSelected
    {
        ProviderId = "lloyds-bank",
        SchemeId = "faster_payments_service"
    },
    Redirect = new Redirect
    {
        ReturnUri = new Uri("https://yourdomain.com/callback"),
        DirectReturnUri = new Uri("https://yourdomain.com/direct-callback")
    }
};

var response = await _client.Payments.StartAuthorizationFlow(
    paymentId,
    idempotencyKey: Guid.NewGuid().ToString(),
    authRequest
);

if (response.IsSuccessful)
{
    var redirectUrl = response.Data.Match(
        authorizing => authorizing.AuthorizationFlow.Actions.Next.Uri.ToString(),
        failed => throw new Exception($"Authorization failed: {failed.FailureReason}")
    );

    return Redirect(redirectUrl);
}
```

## Idempotency

Always use idempotency keys to prevent duplicate payments:

```csharp
// Generate once per operation
var idempotencyKey = Guid.NewGuid().ToString();

// Safe to retry with same key
var response = await _client.Payments.CreatePayment(request, idempotencyKey);
if (!response.IsSuccessful)
{
    // Retry with SAME key - won't create duplicate
    response = await _client.Payments.CreatePayment(request, idempotencyKey);
}
```

## Best Practices

### 1. Always Use Idempotency Keys

```csharp
// Store with your order/transaction
public class PaymentTransaction
{
    public string OrderId { get; set; }
    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();
    public string PaymentId { get; set; }
}
```

### 2. Handle All Payment Statuses

```csharp
public async Task<PaymentStatus> CheckPaymentStatus(string paymentId)
{
    var response = await _client.Payments.GetPayment(paymentId);

    if (!response.IsSuccessful)
    {
        _logger.LogError("Failed to get payment: {TraceId}", response.TraceId);
        throw new Exception("Payment lookup failed");
    }

    return response.Data.Match<PaymentStatus>(
        authRequired => PaymentStatus.AwaitingAuth,
        authorizing => PaymentStatus.Authorizing,
        authorized => PaymentStatus.Authorized,
        executed => PaymentStatus.Executed,
        settled => PaymentStatus.Settled,
        failed => PaymentStatus.Failed,
        attemptFailed => PaymentStatus.AttemptFailed
    );
}
```

### 3. Use Webhooks for Status Updates

Don't poll - use webhooks to get notified of status changes:

```csharp
[HttpPost("webhooks/payments")]
public async Task<IActionResult> HandlePaymentWebhook([FromBody] WebhookPayload payload)
{
    // Verify webhook signature
    // Process payment status change
    var paymentId = payload.PaymentId;
    var newStatus = payload.Status;

    await UpdatePaymentStatus(paymentId, newStatus);

    return Ok();
}
```

### 4. Store Payment Details

```csharp
public class PaymentRecord
{
    public string PaymentId { get; set; }
    public string IdempotencyKey { get; set; }
    public int AmountInMinor { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
}
```

### 5. Handle Failures Gracefully

```csharp
if (!response.IsSuccessful)
{
    _logger.LogError(
        "Payment creation failed. TraceId: {TraceId}, Status: {Status}, Error: {Error}",
        response.TraceId,
        response.StatusCode,
        response.Problem?.Detail
    );

    // Present user-friendly message
    return View("PaymentError", new ErrorModel
    {
        Message = "Payment could not be initiated. Please try again.",
        SupportReference = response.TraceId
    });
}
```

## Common Scenarios

### E-commerce Checkout

```csharp
public async Task<string> ProcessCheckoutPayment(Order order)
{
    var request = new CreatePaymentRequest(
        amountInMinor: order.TotalInMinor,
        currency: Currencies.GBP,
        paymentMethod: new CreatePaymentMethod.BankTransfer(
            new CreateProviderSelection.UserSelected(),
            new Beneficiary.MerchantAccount(merchantAccountId)
        ),
        user: new PaymentUserRequest(order.CustomerName, order.CustomerEmail),
        hostedPage: new HostedPageRequest(
            returnUri: new Uri($"https://shop.com/orders/{order.Id}/complete"),
            countryCode: "GB",
            languageCode: "en"
        )
    );

    var response = await _client.Payments.CreatePayment(
        request,
        idempotencyKey: order.PaymentIdempotencyKey
    );

    // Store payment ID with order
    order.PaymentId = response.Data!.Id;
    await _orderRepository.UpdateAsync(order);

    return response.Data!.Match(
        authRequired => authRequired.HostedPage!.Uri.ToString(),
        _ => throw new Exception("Unexpected payment state")
    );
}
```

### Subscription Payment

```csharp
// Use mandate for recurring payments - see Mandates guide
```

### Refund Processing

```csharp
public async Task<bool> ProcessRefund(string paymentId, int amountInMinor, string reason)
{
    var refundRequest = new CreatePaymentRefundRequest(
        amountInMinor: amountInMinor,
        reference: $"Refund: {reason}"
    );

    var response = await _client.Payments.CreatePaymentRefund(
        paymentId,
        idempotencyKey: Guid.NewGuid().ToString(),
        refundRequest
    );

    if (response.IsSuccessful)
    {
        _logger.LogInformation(
            "Refund created: {RefundId} for payment {PaymentId}",
            response.Data.Id,
            paymentId
        );
        return true;
    }

    _logger.LogError(
        "Refund failed for payment {PaymentId}: {Error}",
        paymentId,
        response.Problem?.Detail
    );
    return false;
}
```

## See Also

- [Mandates](mandates.md) - For recurring payments
- [Error Handling](error-handling.md) - Handle payment errors
- [API Reference](xref:TrueLayer.Payments.IPaymentsApi) - Complete API documentation
