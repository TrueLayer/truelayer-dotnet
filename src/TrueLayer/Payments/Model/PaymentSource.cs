using OneOf;

namespace TrueLayer.Payments.Model
{
    using AccountIdentifiersUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Bban,
        AccountIdentifier.Iban,
        AccountIdentifier.Nrb
    >;

    /// <summary>
    /// Represents a payment source
    /// </summary>
    /// <param name="Id">The unique identifier of the payment source</param>
    /// <param name="AccountIdentifiers">The identifiers for the account associated with the payment source</param>
    /// <param name="AccountHolderName">The name of the payment source account holder</param>
    public record PaymentSource(string Id, AccountIdentifiersUnion[] AccountIdentifiers, string AccountHolderName);
}
