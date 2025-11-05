# Payments

Create and manage payments using the TrueLayer Payments API.

## Creating a Payment

```csharp
var request = new CreatePaymentRequest(
    amountInMinor: 100,
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
```

## Retrieving a Payment

```csharp
var response = await _client.Payments.GetPayment(paymentId);
```

## Payment Refunds

### Create a Refund

```csharp
var refundRequest = new CreatePaymentRefundRequest(
    amountInMinor: 50,
    reference: "Refund for order #123"
);

var response = await _client.Payments.CreatePaymentRefund(
    paymentId,
    idempotencyKey: Guid.NewGuid().ToString(),
    refundRequest
);
```

### List Refunds

```csharp
var response = await _client.Payments.ListPaymentRefunds(paymentId);
```

### Get Specific Refund

```csharp
var response = await _client.Payments.GetPaymentRefund(paymentId, refundId);
```

## Cancelling a Payment

```csharp
var response = await _client.Payments.CancelPayment(
    paymentId,
    idempotencyKey: Guid.NewGuid().ToString()
);
```

## See Also

- [API Reference](xref:TrueLayer.Payments.IPaymentsApi)
- [Error Handling](error-handling.md)
