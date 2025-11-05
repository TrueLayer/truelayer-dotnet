# Merchant Accounts

Manage your TrueLayer merchant accounts.

## Listing Merchant Accounts

```csharp
var response = await _client.MerchantAccounts.ListMerchantAccounts();
```

## Getting Account Details

```csharp
var response = await _client.MerchantAccounts.GetMerchantAccount(accountId);
```

## See Also

- [API Reference](xref:TrueLayer.MerchantAccounts.IMerchantAccountsApi)
- [Payouts](payouts.md)
