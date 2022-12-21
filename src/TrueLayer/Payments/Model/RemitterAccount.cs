using OneOf;

namespace TrueLayer.Payments.Model
{
    using AccountIdentifierUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Iban,
        AccountIdentifier.Bban,
        AccountIdentifier.Nrb>;

    /// <summary>
    /// Represent a remitter account
    /// </summary>
    /// <param name="AccountHolderName">The name of the remitter account holder</param>
    /// <param name="AccountIdentifier">A unique account identifier for the remitter account</param>
    public record RemitterAccount(string AccountHolderName, AccountIdentifierUnion AccountIdentifier);
}
