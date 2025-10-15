using System.Collections.Generic;
using System.Text.Json.Serialization;
using OneOf;
using TrueLayer.Common;
using TrueLayer.Serialization;
using static TrueLayer.Payouts.Model.AccountIdentifier;

namespace TrueLayer.Payouts.Model
{
    using AccountIdentifierUnion = OneOf<Iban, SortCodeAccountNumber, Nrb>;

    /// <summary>
    /// Beneficiary types for GET payout responses
    /// </summary>
    public static class GetPayoutBeneficiary
    {
        /// <summary>
        /// Represents a TrueLayer beneficiary merchant account (GET response)
        /// </summary>
        [JsonDiscriminator("payment_source")]
        public sealed record PaymentSource : IDiscriminated
        {
            const string Discriminator = "payment_source";

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => Discriminator;

            /// <summary>
            /// Gets the ID of the external account which has become a payment source
            /// </summary>
            public string PaymentSourceId { get; init; } = null!;

            /// <summary>
            /// Gets the ID of the owning user of the external account
            /// </summary>
            public string UserId { get; init; } = null!;

            /// <summary>
            /// Gets the reference for the payout
            /// </summary>
            public string Reference { get; init; } = null!;

            /// <summary>
            /// Gets the name of the account holder
            /// </summary>
            public string AccountHolderName { get; init; } = null!;

            /// <summary>
            /// Gets the account identifiers
            /// </summary>
            public List<AccountIdentifierUnion> AccountIdentifiers { get; init; } = null!;
        }

        /// <summary>
        /// Represents an external beneficiary account
        /// </summary>
        [JsonDiscriminator("external_account")]
        public sealed record ExternalAccount : IDiscriminated
        {
            const string Discriminator = "external_account";

            public string Type => Discriminator;
            public string AccountHolderName { get; init; } = null!;
            public string Reference { get; init; } = null!;
            public AccountIdentifierUnion AccountIdentifier { get; init; }
        }

        /// <summary>
        /// Represents a client's preconfigured business account
        /// </summary>
        [JsonDiscriminator("business_account")]
        public sealed record BusinessAccount : IDiscriminated
        {
            const string Discriminator = "business_account";

            public string Type => Discriminator;
            public string? AccountHolderName { get; init; }
            public string Reference { get; init; } = null!;
            public AccountIdentifierUnion? AccountIdentifier { get; init; }
        }

        /// <summary>
        /// Represents a user-determined beneficiary
        /// </summary>
        [JsonDiscriminator("user_determined")]
        public sealed record UserDetermined : IDiscriminated
        {
            const string Discriminator = "user_determined";

            public string Type => Discriminator;

            /// <summary>
            /// Gets the reference for the payout, which displays in the beneficiary's bank statement
            /// </summary>
            public string Reference { get; init; } = null!;

            /// <summary>
            /// Gets the user details of the beneficiary
            /// </summary>
            public PayoutUserResponse User { get; init; } = null!;

            /// <summary>
            /// Gets the name of the account holder
            /// </summary>
            public string? AccountHolderName { get; init; }

            /// <summary>
            /// Gets the account identifiers
            /// </summary>
            public List<AccountIdentifierUnion>? AccountIdentifiers { get; init; }

            /// <summary>
            /// Gets the verification details that were requested for the payout
            /// </summary>
            public Verification? Verification { get; init; }
        }
    }
}
