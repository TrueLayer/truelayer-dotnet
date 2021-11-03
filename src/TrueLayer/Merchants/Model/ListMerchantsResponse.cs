namespace TrueLayer.Merchants.Model
{
    using System.Collections.Generic;

    public record ListMerchantsResponse(IEnumerable<MerchantAccount> items);

    public record MerchantAccount(
        string id, 
        string currency, 
        IEnumerable<object> scheme_identifiers,
        long available_balance_in_minor, 
        long current_balance_in_minor, 
        string account_holder_name);
}
