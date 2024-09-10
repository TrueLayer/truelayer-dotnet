using System;
using System.Text.Json.Serialization;
using OneOf;
using TrueLayer.Common;
using TrueLayer.Serialization;
using static TrueLayer.Payouts.Model.AccountIdentifier;
using static TrueLayer.Payouts.Model.Beneficiary;

namespace TrueLayer.Payouts.Model
{
    using AccountIdentifierUnion = OneOf<Iban, SortCodeAccountNumber>;

    [GenerateOneOf]
    public partial class PayoutBeneficiaryUnion
        : OneOfBase<PaymentSource, ExternalAccount, BusinessAccount, UserDetermined>
    { }

    public static class Beneficiary
    {
        /// <summary>
        /// Represents a TrueLayer beneficiary merchant account
        /// </summary>
        [JsonDiscriminator(Discriminator)]
        public sealed record PaymentSource : IDiscriminated
        {
            const string Discriminator = "payment_source";

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
            public string Type => Discriminator;

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
        [JsonDiscriminator(Discriminator)]
        public sealed record ExternalAccount : IDiscriminated
        {
            const string Discriminator = "external_account";

            public ExternalAccount(
                string accountHolderName,
                string reference,
                AccountIdentifierUnion accountIdentifier,
                DateTime? dateOfBirth = null,
                Address? address = null)
            {
                AccountHolderName = accountHolderName.NotNullOrWhiteSpace(nameof(accountHolderName));
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
                AccountIdentifier = accountIdentifier;
                DateOfBirth = dateOfBirth;
                Address = address;
            }

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => Discriminator;

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

            /// <summary>
            /// Gets the user's date of birth.
            /// </summary>
            [JsonConverter(typeof(DateTimeDateOnlyJsonConverter))]
            public DateTime? DateOfBirth { get; }

            /// <summary>
            /// Gets the user's physical address
            /// </summary>
            public Address? Address { get; }
        }

        /// <summary>
        /// Represents a client's preconfigured business account.
        /// </summary>
        [JsonDiscriminator(Discriminator)]
        public sealed record BusinessAccount : IDiscriminated
        {
            const string Discriminator = "business_account";
            /// <summary>
            /// Creates a new <see cref="BusinessAccount"/>.
            /// </summary>
            /// <param name="reference">A reference for the payout.</param>
            public BusinessAccount(string reference)
            {
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
            }

            /// <summary>
            /// Creates a new <see cref="BusinessAccount"/>.
            /// </summary>
            /// <param name="reference">A reference for the payout.</param>
            /// <param name="accountHolderName">The business account holder name</param>
            /// <param name="accountIdentifier">The unique identifier for the business account</param>
            public BusinessAccount(string reference, string accountHolderName, AccountIdentifierUnion accountIdentifier)
            {
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
                AccountHolderName = accountHolderName;
                AccountIdentifier = accountIdentifier;
            }

            /// <summary>
            /// Gets the type of beneficiary
            /// </summary>
            public string Type => Discriminator;

            /// <summary>
            /// Gets the name of the business account holder
            /// </summary>
            public string AccountHolderName { get; } = "";

            /// <summary>
            /// Gets the reference for the business bank account holder
            /// </summary>
            public string Reference { get; }

            /// <summary>
            /// Gets the unique scheme identifier for the business account
            /// </summary>
            public AccountIdentifierUnion AccountIdentifier { get; }
        }

        [JsonDiscriminator(Discriminator)]
        public sealed record UserDetermined : IDiscriminated
        {
            const string Discriminator = "user_determined";

            public UserDetermined(string reference, string accountHolderName, AccountIdentifierUnion accountIdentifier)
            {
                AccountHolderName = accountHolderName;
                AccountIdentifier = accountIdentifier;
                Reference = reference.NotNullOrWhiteSpace(nameof(reference));
            }

            public string Type => Discriminator;

            /// <summary>
            /// Gets the name of the external account holder
            /// </summary>
            public string AccountHolderName { get; } = "";

            /// <summary>
            /// Gets the reference for the external bank account holder
            /// </summary>
            public string Reference { get; }

            /// <summary>
            /// Gets the unique scheme identifier for the external account
            /// </summary>
            public AccountIdentifierUnion AccountIdentifier { get; }
        }
    }
}
