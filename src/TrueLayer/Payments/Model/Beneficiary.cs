using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.AccountIdentifier;

namespace TrueLayer.Payments.Model
{
    using AccountIdentifierUnion = OneOf<SortCodeAccountNumber, Iban>;

    public static class Beneficiary
    {
        /// <summary>
        /// Represents a TrueLayer beneficiary merchant account
        /// </summary>
        [JsonDiscriminator("merchant_account")]
        public sealed record MerchantAccount : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="MerchantAccount"/>
            /// </summary>
            /// <param name="merchantAccountId">The TrueLayer merchant account identifier</param>
            public MerchantAccount(string merchantAccountId)
            {
                MerchantAccountId = merchantAccountId.NotNullOrWhiteSpace(nameof(merchantAccountId));
            }

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => "merchant_account";

            /// <summary>
            /// Gets the TrueLayer merchant account identifier
            /// </summary>
            public string MerchantAccountId { get; }

            /// <summary>
            /// Gets or inits the name of the beneficiary.
            /// If unspecified, the API will use the account owner name associated to the selected merchant account.
            /// </summary>
            public string? AccountHolderName { get; init; }

            /// <summary>
            /// Gets or inits A reference for the payment. Not visible to the remitter.
            /// </summary>
            public string? Reference { get; init; }
        }

        /// <summary>
        /// Represents an external beneficiary account
        /// </summary>
        [JsonDiscriminator("external_account")]
        public sealed record ExternalAccount : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="ExternalAccount"/>
            /// </summary>
            /// <param name="accountHolderName">The external account holder name</param>
            /// <param name="reference">The reference for the external bank account holder</param>
            /// <param name="accountIdentifier">The unique identifier for the external account</param>
            public ExternalAccount(string accountHolderName, string reference, AccountIdentifierUnion accountIdentifier)
            {
                AccountHolderName = accountHolderName.NotNullOrWhiteSpace(nameof(accountHolderName));
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
                AccountIdentifier = accountIdentifier;
            }

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => "external_account";

            /// <summary>
            /// Gets the name of the external account holder
            /// </summary>
            public string AccountHolderName { get; }

            /// <summary>
            /// Gets the reference for the external bank account holder
            /// </summary>
            public string Reference { get; }

            /// <summary>
            /// Gets the unique identifier for the external account
            /// </summary>
            public AccountIdentifierUnion AccountIdentifier { get; }
        }
    }
}
