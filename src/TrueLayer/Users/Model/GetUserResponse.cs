using OneOf;
using TrueLayer.Payments.Model;

namespace TrueLayer.Users.Model
{
    using SchemeIdentifiersUnion = OneOf<
        SchemeIdentifier.SortCodeAccountNumber,
        SchemeIdentifier.Bban,
        SchemeIdentifier.Iban,
        SchemeIdentifier.Nrb
    >;

    /// <summary>
    /// Represents an end user details.
    /// </summary>
    /// <param name="Id">The unique identifier of the user.</param>
    /// <param name="ExternalAccounts">Details of the external accounts of the user.</param>
    /// <param name="Name">The user's name.</param>
    /// <param name="Email">The user's email address. Either an email address or <param ref="phone"/> must be provided.</param>
    /// <param name="Phone">The user's phone number. Either a phone number or <param ref="email"/> must be provided.</param>
    public record GetUserResponse(string Id, string Name, UserExternalAccount[] ExternalAccounts , string? Email = null, string? Phone = null);

    /// <summary>
    /// Represents an external account.
    /// </summary>
    /// <param name="Id">TrueLayer unique identifier of the external account</param>
    /// <param name="Name">Account holder name</param>
    /// <param name="SchemeIdentifiers">The scheme identifiers for the external account</param>
    public record UserExternalAccount(string Id, string Name, SchemeIdentifiersUnion[] SchemeIdentifiers);
}
