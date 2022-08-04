using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payouts.Model.AccountIdentifier;

namespace TrueLayer.Payouts.Model
{
    using AccountIdentifierUnion = OneOf<Iban>;

    public static class Beneficiary
    {
        /// <summary>
        /// Represents a TrueLayer beneficiary merchant account
        /// </summary>
        [JsonDiscriminator("payment_source")]
        public sealed record PaymentSource : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="PaymentSource"/>.
            /// </summary>
            /// <param name="paymentSourceId">ID of the external account which has become a payment source.</param>
            /// <param name="userId">ID of the owning user of the external account.</param>
            /// <param name="reference">A reference for the payout.</param>
            public PaymentSource(string paymentSourceId, string userId, string reference)
            {
                PaymentSourceId = paymentSourceId.NotNullOrWhiteSpace(nameof(paymentSourceId));
                UserId = userId.NotNullOrWhiteSpace(nameof(userId));
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
            }

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => "payment_source";

            /// <summary>
            /// Gets the ID of the external account which has become a payment source
            /// </summary>
            public string PaymentSourceId { get; }

            /// <summary>
            /// Gets the ID of the owning user of the external account.
            /// </summary>
            public string UserId { get; }

            /// <summary>
            /// Gets a reference for the payout.
            /// </summary>
            public string Reference { get; }
        }

        /// <summary>
        /// Represents an external beneficiary account
        /// </summary>
        [JsonDiscriminator("external_account")]
        public sealed record ExternalAccount : IDiscriminated
        {
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
            /// Gets the unique scheme identifier for the external account
            /// </summary>
            public AccountIdentifierUnion AccountIdentifier { get; }
        }

        /// <summary>
        /// Represent's a client's preconfigured business account.
        /// </summary>
        public sealed record BusinessAccount : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="BusinessAccount"/>.
            /// </summary>
            /// <param name="reference">A reference for the payout.</param>
            public BusinessAccount(string reference)
            {
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
            }

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => "business_account";

            /// <summary>
            /// Gets a reference for the payout.
            /// </summary>
            public string Reference { get; }
        }
    }
}
