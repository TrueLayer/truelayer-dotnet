# Payouts

Process payouts to beneficiaries using the TrueLayer Payouts API.

## Creating a Payout

```csharp
var request = new CreatePayoutRequest(
    merchantAccountId: "your-merchant-account-id",
    amountInMinor: 10000,
    currency: Currencies.GBP,
    beneficiary: new Beneficiary.ExternalAccount(
        "Beneficiary Name",
        "reference",
        new AccountIdentifier.Iban("GB33BUKB20201555555555")
    )
);

var response = await _client.Payouts.CreatePayout(
    request,
    idempotencyKey: Guid.NewGuid().ToString()
);
```

## Retrieving a Payout

```csharp
var response = await _client.Payouts.GetPayout(payoutId);
```

## See Also

- [API Reference](xref:TrueLayer.Payouts.IPayoutsApi)
- [Merchant Accounts](merchant-accounts.md)
