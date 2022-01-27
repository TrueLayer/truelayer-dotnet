using OneOf;
using TrueLayer.Payments.Model;

namespace TrueLayer.MerchantAccounts.Model
{
    using SchemeIdentifiersUnion = OneOf<
        SchemeIdentifier.SortCodeAccountNumber,
        SchemeIdentifier.Bban,
        SchemeIdentifier.Iban,
        SchemeIdentifier.Nrb
    >;

    /// <summary>
    /// Represents an end user's external accounts details.
    /// </summary>
    /// <param name="Items">Details of the external accounts of the user.</param>
    public record GetUserPaymentSourcesResponse(UserPaymentSource[] Items);

    /// <summary>
    /// Represents an external account.
    /// </summary>
    /// <param name="Id">TrueLayer unique identifier of the external account</param>
    /// <param name="Name">Account holder name</param>
    /// <param name="SchemeIdentifiers">The scheme identifiers for the external account</param>
    public record UserPaymentSource(string Id, string Name, SchemeIdentifiersUnion[] SchemeIdentifiers);
}
