using System.Collections.Generic;
using OneOf;
using AccountIdentifier = TrueLayer.Payments.Model.AccountIdentifier;

namespace TrueLayer.MerchantAccounts.Model;

using AccountIdentifiersUnion = OneOf<
    AccountIdentifier.SortCodeAccountNumber,
    AccountIdentifier.Bban,
    AccountIdentifier.Iban,
    AccountIdentifier.Nrb
>;

/// <summary>
/// Represents a TrueLayer merchant account with balance and identification information.
/// </summary>
/// <param name="Id">The unique identifier for the merchant account.</param>
/// <param name="Currency">The currency code for the account.</param>
/// <param name="AccountIdentifiers">The list of account identifiers (sort code/account number, IBAN, etc.).</param>
/// <param name="AvailableBalanceInMinor">The available balance in minor currency units.</param>
/// <param name="CurrentBalanceInMinor">The current balance in minor currency units.</param>
/// <param name="AccountHolderName">The name of the account holder.</param>
public record MerchantAccount(
    string Id,
    string Currency,
    IEnumerable<AccountIdentifiersUnion> AccountIdentifiers,
    long AvailableBalanceInMinor,
    long CurrentBalanceInMinor,
    string AccountHolderName);