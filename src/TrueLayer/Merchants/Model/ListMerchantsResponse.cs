using System.Collections.Generic;

namespace TrueLayer.Merchants.Model
{
    public record ListMerchantsResponse(IEnumerable<MerchantAccount> Items);

    public record MerchantAccount(
        string Id, 
        string Currency, 
        IEnumerable<object> SchemeIdentifiers,
        long AvailableBalanceInMinor, 
        long CurrentBalanceInMinor, 
        string AccountHolderName);
}
