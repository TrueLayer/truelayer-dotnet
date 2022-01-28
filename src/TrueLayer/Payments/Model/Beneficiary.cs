using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.AccountIdentifier;

namespace TrueLayer.Payments.Model
{
    using AccountIdentifierUnion = OneOf<SortCodeAccountNumber>;

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
            /// <param name="id">The TrueLayer merchant account identifier</param>
            public MerchantAccount(string id)
            {
                Id = id.NotNullOrWhiteSpace(nameof(id));
            }

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => "merchant_account";

            /// <summary>
            /// Gets the TrueLayer merchant account identifier
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// The name of the beneficiary.
            /// If unspecified, the API will use the account owner name associated to the selected merchant account.
            /// </summary>
            public string? Name { get; init; }
        }

        /// <summary>
        /// Represents an external beneficiary account
        /// </summary>
        [JsonDiscriminator("external_account")]
        public sealed record ExternalAccount : IDiscriminated
        {
            public ExternalAccount(string name, string reference, AccountIdentifierUnion accountIdentifier)
            {
                Name = name.NotNullOrWhiteSpace(nameof(name));
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
            public string Name { get; }

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
