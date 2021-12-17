using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.SchemeIdentifier;

namespace TrueLayer.Payouts.Model
{
    using SchemeIdentifierUnion = OneOf<SortCodeAccountNumber>;

    public static class Beneficiary
    {
        /// <summary>
        /// Represents a TrueLayer beneficiary merchant account
        /// </summary>
        [JsonDiscriminator("payment_source")]
        public sealed record PaymentSource : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="PaymentSource"/>
            /// </summary>
            /// <param name="id">The TrueLayer merchant account identifier</param>
            public PaymentSource(string id)
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
            public ExternalAccount(string name, string reference, SchemeIdentifierUnion schemeIdentifier)
            {
                Name = name.NotNullOrWhiteSpace(nameof(name));
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
                SchemeIdentifier = schemeIdentifier;
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
            /// Gets the unique scheme identifier for the external account
            /// </summary>
            public SchemeIdentifierUnion SchemeIdentifier { get; }
        }
    }
}
