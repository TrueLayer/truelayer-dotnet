using OneOf;
using SchemeIdentifier = TrueLayer.Payments.Model.SchemeIdentifier;
using System.Collections.Generic;

namespace TrueLayer.MerchantAccounts.Model
{
    using SchemeIdentifiersUnion = OneOf<
        SchemeIdentifier.SortCodeAccountNumber,
        SchemeIdentifier.Bban,
        SchemeIdentifier.Iban,
        SchemeIdentifier.Nrb
    >;

    public record MerchantAccount(
        string Id, 
        string Currency, 
        IEnumerable<SchemeIdentifiersUnion> SchemeIdentifiers,
        long AvailableBalanceInMinor, 
        long CurrentBalanceInMinor, 
        string AccountHolderName);
}
