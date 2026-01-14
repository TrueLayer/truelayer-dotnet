# Merchant Accounts

Manage your TrueLayer merchant accounts for receiving payments and processing payouts. Merchant accounts are the destination accounts where funds from payments are settled.

## Understanding Merchant Accounts

Merchant accounts are TrueLayer-managed accounts that:
- **Receive payments**: Funds from customer payments are settled here
- **Source payouts**: Transfer funds to beneficiaries (sellers, suppliers, refunds)
- **Multi-currency**: Support different currencies (GBP, EUR, etc.)
- **Account identifiers**: Have sort codes, account numbers, or IBANs for transfers

## Listing Merchant Accounts

Retrieve all your merchant accounts:

```csharp
var response = await _client.MerchantAccounts.ListMerchantAccounts();

if (response.IsSuccessful)
{
    foreach (var account in response.Data.Items)
    {
        Console.WriteLine($"Account ID: {account.Id}");
        Console.WriteLine($"Currency: {account.Currency}");
        Console.WriteLine($"Available Balance: {account.AvailableBalanceInMinor / 100.0:C}");
        Console.WriteLine($"Current Balance: {account.CurrentBalanceInMinor / 100.0:C}");

        // Account identifiers
        foreach (var identifier in account.AccountIdentifiers)
        {
            identifier.Match(
                sortCode => Console.WriteLine($"  Sort Code: {sortCode.SortCode}, Account: {sortCode.AccountNumber}"),
                iban => Console.WriteLine($"  IBAN: {iban.Iban}"),
                scan => Console.WriteLine($"  SCAN: {scan.Scan}")
            );
        }
    }
}
```

## Getting Account Details

Get detailed information about a specific merchant account:

```csharp
var response = await _client.MerchantAccounts.GetMerchantAccount(accountId);

if (response.IsSuccessful)
{
    var account = response.Data;

    Console.WriteLine($"Account ID: {account.Id}");
    Console.WriteLine($"Currency: {account.Currency}");
    Console.WriteLine($"Available Balance: {account.AvailableBalanceInMinor}");
    Console.WriteLine($"Current Balance: {account.CurrentBalanceInMinor}");

    // Display account identifiers
    foreach (var identifier in account.AccountIdentifiers)
    {
        identifier.Match(
            sortCode =>
            {
                Console.WriteLine("UK Account:");
                Console.WriteLine($"  Sort Code: {sortCode.SortCode}");
                Console.WriteLine($"  Account Number: {sortCode.AccountNumber}");
            },
            iban =>
            {
                Console.WriteLine("IBAN Account:");
                Console.WriteLine($"  IBAN: {iban.Iban}");
            },
            scan =>
            {
                Console.WriteLine("SCAN Account:");
                Console.WriteLine($"  SCAN: {scan.Scan}");
            }
        );
    }
}
```

## Account Balances

### Understanding Balance Types

Merchant accounts have two balance types:

- **Current Balance**: Total funds in the account, including pending transactions
- **Available Balance**: Funds available for immediate payout (excludes holds, reserves)

```csharp
public class MerchantAccountBalance
{
    public string AccountId { get; set; }
    public string Currency { get; set; }
    public long CurrentBalanceInMinor { get; set; }
    public long AvailableBalanceInMinor { get; set; }

    public decimal CurrentBalance => CurrentBalanceInMinor / 100m;
    public decimal AvailableBalance => AvailableBalanceInMinor / 100m;
    public decimal PendingBalance => CurrentBalance - AvailableBalance;
}

public async Task<MerchantAccountBalance> GetAccountBalance(string accountId)
{
    var response = await _client.MerchantAccounts.GetMerchantAccount(accountId);

    if (!response.IsSuccessful)
    {
        throw new Exception($"Failed to get account: {response.Problem?.Detail}");
    }

    return new MerchantAccountBalance
    {
        AccountId = response.Data.Id,
        Currency = response.Data.Currency,
        CurrentBalanceInMinor = response.Data.CurrentBalanceInMinor,
        AvailableBalanceInMinor = response.Data.AvailableBalanceInMinor
    };
}
```

### Check Available Funds

Verify sufficient funds before creating a payout:

```csharp
public async Task<bool> HasSufficientFunds(string accountId, long amountInMinor)
{
    var response = await _client.MerchantAccounts.GetMerchantAccount(accountId);

    if (!response.IsSuccessful)
    {
        return false;
    }

    return response.Data.AvailableBalanceInMinor >= amountInMinor;
}

// Usage
if (await HasSufficientFunds(merchantAccountId, payoutAmount))
{
    // Create payout
    var payoutResponse = await _client.Payouts.CreatePayout(
        payoutRequest,
        idempotencyKey: Guid.NewGuid().ToString()
    );
}
else
{
    _logger.LogWarning("Insufficient funds for payout of {Amount}", payoutAmount);
}
```

## Multi-Currency Account Management

### Managing Multiple Currencies

```csharp
public class CurrencyAccountManager
{
    private readonly ITrueLayerClient _client;

    public CurrencyAccountManager(ITrueLayerClient client)
    {
        _client = client;
    }

    public async Task<Dictionary<string, MerchantAccount>> GetAccountsByCurrency()
    {
        var response = await _client.MerchantAccounts.ListMerchantAccounts();

        if (!response.IsSuccessful)
        {
            return new Dictionary<string, MerchantAccount>();
        }

        return response.Data.Items
            .GroupBy(a => a.Currency)
            .ToDictionary(
                g => g.Key,
                g => g.First() // Take first account for each currency
            );
    }

    public async Task<string> GetAccountIdForCurrency(string currency)
    {
        var accounts = await GetAccountsByCurrency();

        if (!accounts.ContainsKey(currency))
        {
            throw new Exception($"No merchant account found for currency {currency}");
        }

        return accounts[currency].Id;
    }
}

// Usage
var manager = new CurrencyAccountManager(_client);
var gbpAccountId = await manager.GetAccountIdForCurrency("GBP");
var eurAccountId = await manager.GetAccountIdForCurrency("EUR");
```

### Currency-Specific Payouts

```csharp
public async Task<string> CreatePayoutInCurrency(
    string currency,
    long amountInMinor,
    Beneficiary beneficiary)
{
    // Get the merchant account for this currency
    var accountId = await GetAccountIdForCurrency(currency);

    // Check available balance
    if (!await HasSufficientFunds(accountId, amountInMinor))
    {
        throw new InsufficientFundsException(
            $"Insufficient {currency} funds for payout"
        );
    }

    // Create payout
    var request = new CreatePayoutRequest(
        merchantAccountId: accountId,
        amountInMinor: amountInMinor,
        currency: currency,
        beneficiary: beneficiary
    );

    var response = await _client.Payouts.CreatePayout(
        request,
        idempotencyKey: Guid.NewGuid().ToString()
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"Payout failed: {response.Problem?.Detail}");
    }

    return response.Data!.Match(
        authRequired => authRequired.Id,
        created => created.Id
    );
}
```

## Account Identifiers

### Working with Account Identifiers

Extract and use account identifiers for external transfers:

```csharp
public class AccountIdentifierHelper
{
    public static (string? SortCode, string? AccountNumber) GetUKDetails(
        MerchantAccount account)
    {
        foreach (var identifier in account.AccountIdentifiers)
        {
            var result = identifier.Match<(string?, string?)>(
                sortCode => (sortCode.SortCode, sortCode.AccountNumber),
                iban => (null, null),
                scan => (null, null)
            );

            if (result.Item1 != null)
            {
                return result;
            }
        }

        return (null, null);
    }

    public static string? GetIBAN(MerchantAccount account)
    {
        foreach (var identifier in account.AccountIdentifiers)
        {
            var iban = identifier.Match<string?>(
                sortCode => null,
                iban => iban.Iban,
                scan => null
            );

            if (iban != null)
            {
                return iban;
            }
        }

        return null;
    }

    public static string GetFormattedIdentifier(MerchantAccount account)
    {
        foreach (var identifier in account.AccountIdentifiers)
        {
            return identifier.Match(
                sortCode => $"{sortCode.SortCode} {sortCode.AccountNumber}",
                iban => iban.Iban,
                scan => scan.Scan
            );
        }

        return "No identifier available";
    }
}
```

## Account Information Display

### Display Account Summary

```csharp
public class MerchantAccountViewModel
{
    public string Id { get; set; }
    public string Currency { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string FormattedIdentifier { get; set; }
    public string AccountType { get; set; } // "UK", "IBAN", or "SCAN"
}

public async Task<List<MerchantAccountViewModel>> GetAccountsForDisplay()
{
    var response = await _client.MerchantAccounts.ListMerchantAccounts();

    if (!response.IsSuccessful)
    {
        return new List<MerchantAccountViewModel>();
    }

    return response.Data.Items
        .Select(account => new MerchantAccountViewModel
        {
            Id = account.Id,
            Currency = account.Currency,
            CurrentBalance = account.CurrentBalanceInMinor / 100m,
            AvailableBalance = account.AvailableBalanceInMinor / 100m,
            FormattedIdentifier = AccountIdentifierHelper.GetFormattedIdentifier(account),
            AccountType = DetermineAccountType(account)
        })
        .ToList();
}

private string DetermineAccountType(MerchantAccount account)
{
    foreach (var identifier in account.AccountIdentifiers)
    {
        return identifier.Match(
            sortCode => "UK",
            iban => "IBAN",
            scan => "SCAN"
        );
    }

    return "Unknown";
}
```

## Payment Receiving

### Using Merchant Accounts as Payment Beneficiaries

Direct payments to your merchant account:

```csharp
public async Task<string> CreatePaymentToMerchantAccount(
    string merchantAccountId,
    int amountInMinor,
    PaymentUserRequest user)
{
    // Get account details for display name
    var accountResponse = await _client.MerchantAccounts.GetMerchantAccount(merchantAccountId);

    if (!accountResponse.IsSuccessful)
    {
        throw new Exception("Merchant account not found");
    }

    var account = accountResponse.Data;

    var request = new CreatePaymentRequest(
        amountInMinor: amountInMinor,
        currency: account.Currency,
        paymentMethod: new CreatePaymentMethod.BankTransfer(
            new CreateProviderSelection.UserSelected(),
            new Beneficiary.MerchantAccount(
                merchantAccountId: merchantAccountId,
                accountHolderName: "Your Business Ltd"
            )
        ),
        user: user
    );

    var response = await _client.Payments.CreatePayment(
        request,
        idempotencyKey: Guid.NewGuid().ToString()
    );

    if (!response.IsSuccessful)
    {
        throw new Exception($"Payment creation failed: {response.Problem?.Detail}");
    }

    return response.Data!.Match(
        authRequired => authRequired.Id,
        authorized => authorized.Id,
        failed => throw new Exception($"Payment failed: {failed.FailureReason}"),
        authorizing => authorizing.Id
    );
}
```

## Best Practices

### 1. Cache Account Information

Merchant account details rarely change - cache them:

```csharp
public class MerchantAccountCache
{
    private readonly ITrueLayerClient _client;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public async Task<MerchantAccount?> GetAccount(string accountId)
    {
        var cacheKey = $"merchant_account_{accountId}";

        if (_cache.TryGetValue(cacheKey, out MerchantAccount cached))
        {
            return cached;
        }

        var response = await _client.MerchantAccounts.GetMerchantAccount(accountId);

        if (!response.IsSuccessful)
        {
            return null;
        }

        _cache.Set(cacheKey, response.Data, CacheDuration);
        return response.Data;
    }

    public async Task<List<MerchantAccount>> GetAllAccounts()
    {
        const string cacheKey = "merchant_accounts_all";

        if (_cache.TryGetValue(cacheKey, out List<MerchantAccount> cached))
        {
            return cached;
        }

        var response = await _client.MerchantAccounts.ListMerchantAccounts();

        if (!response.IsSuccessful)
        {
            return new List<MerchantAccount>();
        }

        var accounts = response.Data.Items.ToList();
        _cache.Set(cacheKey, accounts, CacheDuration);

        return accounts;
    }
}
```

### 2. Always Check Available Balance

Don't rely on current balance - use available balance:

```csharp
public async Task<bool> CanProcessPayout(string accountId, long amountInMinor)
{
    var response = await _client.MerchantAccounts.GetMerchantAccount(accountId);

    if (!response.IsSuccessful)
    {
        return false;
    }

    // Use AvailableBalanceInMinor, not CurrentBalanceInMinor
    return response.Data.AvailableBalanceInMinor >= amountInMinor;
}
```

### 3. Handle Multiple Accounts per Currency

Some merchants may have multiple accounts for the same currency:

```csharp
public async Task<List<MerchantAccount>> GetAccountsForCurrency(string currency)
{
    var response = await _client.MerchantAccounts.ListMerchantAccounts();

    if (!response.IsSuccessful)
    {
        return new List<MerchantAccount>();
    }

    return response.Data.Items
        .Where(a => a.Currency == currency)
        .ToList();
}
```

### 4. Log Balance Changes

Track balance changes for reconciliation:

```csharp
public class BalanceChangeLogger
{
    private readonly ITrueLayerClient _client;
    private readonly ILogger<BalanceChangeLogger> _logger;

    public async Task LogBalanceChange(string accountId, string operation)
    {
        var response = await _client.MerchantAccounts.GetMerchantAccount(accountId);

        if (response.IsSuccessful)
        {
            _logger.LogInformation(
                "Balance after {Operation}: Account {AccountId}, Available: {Available}, Current: {Current}",
                operation,
                accountId,
                response.Data.AvailableBalanceInMinor,
                response.Data.CurrentBalanceInMinor
            );
        }
    }
}

// Usage
await _balanceLogger.LogBalanceChange(accountId, "payout_created");
```

### 5. Store Account Configuration

Store account IDs in configuration:

```csharp
public class MerchantAccountConfiguration
{
    public string PrimaryGBPAccountId { get; set; }
    public string PrimaryEURAccountId { get; set; }
    public Dictionary<string, string> CurrencyAccounts { get; set; }
}

// In appsettings.json
{
  "MerchantAccounts": {
    "PrimaryGBPAccountId": "your-gbp-account-id",
    "PrimaryEURAccountId": "your-eur-account-id",
    "CurrencyAccounts": {
      "GBP": "your-gbp-account-id",
      "EUR": "your-eur-account-id"
    }
  }
}
```

## Common Scenarios

### Account Selection for Payout

```csharp
public async Task<string> ProcessPayoutWithAccountSelection(
    long amountInMinor,
    string currency,
    Beneficiary beneficiary)
{
    // Get all accounts for the currency
    var accounts = await GetAccountsForCurrency(currency);

    if (!accounts.Any())
    {
        throw new Exception($"No {currency} merchant account available");
    }

    // Find account with sufficient balance
    MerchantAccount? selectedAccount = null;

    foreach (var account in accounts)
    {
        if (account.AvailableBalanceInMinor >= amountInMinor)
        {
            selectedAccount = account;
            break;
        }
    }

    if (selectedAccount == null)
    {
        throw new InsufficientFundsException(
            $"Insufficient {currency} funds across all accounts"
        );
    }

    // Create payout with selected account
    var request = new CreatePayoutRequest(
        merchantAccountId: selectedAccount.Id,
        amountInMinor: amountInMinor,
        currency: currency,
        beneficiary: beneficiary
    );

    var response = await _client.Payouts.CreatePayout(
        request,
        idempotencyKey: Guid.NewGuid().ToString()
    );

    return response.Data!.Match(
        authRequired => authRequired.Id,
        created => created.Id
    );
}
```

### Account Balance Dashboard

```csharp
public class AccountDashboard
{
    public class AccountSummary
    {
        public string Currency { get; set; }
        public decimal TotalAvailable { get; set; }
        public decimal TotalCurrent { get; set; }
        public int AccountCount { get; set; }
        public List<MerchantAccount> Accounts { get; set; }
    }

    public async Task<List<AccountSummary>> GetDashboardData()
    {
        var response = await _client.MerchantAccounts.ListMerchantAccounts();

        if (!response.IsSuccessful)
        {
            return new List<AccountSummary>();
        }

        return response.Data.Items
            .GroupBy(a => a.Currency)
            .Select(g => new AccountSummary
            {
                Currency = g.Key,
                TotalAvailable = g.Sum(a => a.AvailableBalanceInMinor) / 100m,
                TotalCurrent = g.Sum(a => a.CurrentBalanceInMinor) / 100m,
                AccountCount = g.Count(),
                Accounts = g.ToList()
            })
            .ToList();
    }
}
```

### Balance Monitoring

```csharp
public class BalanceMonitor
{
    private readonly ITrueLayerClient _client;
    private readonly ILogger<BalanceMonitor> _logger;

    public async Task CheckLowBalances(decimal thresholdPercentage = 0.1m)
    {
        var response = await _client.MerchantAccounts.ListMerchantAccounts();

        if (!response.IsSuccessful)
        {
            return;
        }

        foreach (var account in response.Data.Items)
        {
            var availableBalance = account.AvailableBalanceInMinor;
            var currentBalance = account.CurrentBalanceInMinor;

            if (currentBalance == 0) continue;

            var availablePercentage = (decimal)availableBalance / currentBalance;

            if (availablePercentage < thresholdPercentage)
            {
                _logger.LogWarning(
                    "Low available balance: Account {AccountId} ({Currency}), Available: {Available}, Current: {Current}",
                    account.Id,
                    account.Currency,
                    availableBalance / 100m,
                    currentBalance / 100m
                );

                // Send alert, notification, etc.
            }
        }
    }
}
```

## See Also

- [Payouts](payouts.md) - Creating payouts from merchant accounts
- [Payments](payments.md) - Receiving payments to merchant accounts
- [API Reference](xref:TrueLayer.MerchantAccounts.IMerchantAccountsApi)
