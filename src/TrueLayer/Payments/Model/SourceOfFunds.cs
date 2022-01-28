using OneOf;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    using AccountIdentifiersUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Bban,
        AccountIdentifier.Iban,
        AccountIdentifier.Nrb
    >;

    /// <summary>
    /// Source of funds types
    /// </summary>
    public static class SourceOfFunds
    {
        /// <summary>
        /// Represents an external account source
        /// </summary>
        /// <param name="AccountIdentifiers">The identifiers for the external account</param>
        [JsonDiscriminator(ExternalAccount.Discriminator)]
        public sealed record ExternalAccount(AccountIdentifiersUnion[] AccountIdentifiers)
        {
            public const string Discriminator = "external_account";

            /// <summary>
            /// Gets the source of funds type
            /// </summary>
            public string Type => Discriminator;


            /// <summary>
            /// Gets the TrueLayer unique identifier of the external account
            /// </summary>
            public string? ExternalAccountId { get; init; }


            /// <summary>
            /// Gets the account holder name
            /// This field is null if the chosen payment method/destination does not provide this information.
            /// If the payment beneficiary is of type <see cref="Beneficiary.MerchantAccount"/>, the account holder name is available once the payment is settled.
            /// </summary>
            public string? AccountHolderName { get; init; }
        }
    }
}
