using System.Collections.Generic;
using OneOf;
using AccountIdentifier = TrueLayer.Payments.Model.AccountIdentifier;

namespace TrueLayer.MerchantAccounts.Model
{
    using AccountIdentifiersUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Bban,
        AccountIdentifier.Iban,
        AccountIdentifier.Nrb
    >;

    public record MerchantAccount(
        string Id,
        string Currency,
        IEnumerable<AccountIdentifiersUnion> AccountIdentifiers,
        long AvailableBalanceInMinor,
        long CurrentBalanceInMinor,
        string AccountHolderName);
}
