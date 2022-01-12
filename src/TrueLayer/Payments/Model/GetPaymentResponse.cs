using System;
using OneOf;
using TrueLayer.Serialization;
using TrueLayer.Users.Model;
using static TrueLayer.Payments.Model.Beneficiary;
using static TrueLayer.Payments.Model.PaymentMethod;

namespace TrueLayer.Payments.Model
{
    using BeneficiaryUnion = OneOf<MerchantAccount, ExternalAccount>;
    using PaymentMethodUnion = OneOf<BankTransfer>;
    using SourceOfFundsUnion = OneOf<SourceOfFunds.ExternalAccount>;

    /// <summary>
    /// Get Payment Response Types
    /// </summary>
    public static class GetPaymentResponse
    {
        /// <summary>
        /// Base class containing common properties for all payment states
        /// </summary>
        /// <value></value>
        public record PaymentDetails
        {
            /// <summary>
            /// Gets the unique identifier of the payment
            /// </summary>
            public string Id { get; init; } = null!;

            /// <summary>
            /// Gets the Amount in the minor currency unit e.g. cents
            /// </summary>
            public long AmountInMinor { get; init; }

            /// <summary>
            /// Gets the three-letter ISO3 alpha currency code
            /// </summary>
            /// <example>EUR</example>
            public string Currency { get; init; } = null!;

            /// <summary>
            /// Gets the status of the payment
            /// </summary>
            public string Status { get; init; } = null!;

            /// <summary>
            /// Gets the data and time the payment was created
            /// </summary>
            /// <value></value>
            public DateTime CreatedAt { get; init; }

            /// <summary>
            /// Gets the beneficiary details
            /// </summary>
            public BeneficiaryUnion Beneficiary { get; init; }

            /// <summary>
            /// Gets the payment method details
            /// </summary>
            public PaymentMethodUnion PaymentMethod { get; init; }

            /// <summary>
            /// Gets the end user details
            /// </summary>
            public PaymentUser User { get; init; } = null!;
        }

        /// <summary>
        /// Represents a payment that requires further authorization
        /// </summary>
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired : PaymentDetails;

        /// <summary>
        /// Represents a payment that is being authorized
        /// </summary>
        [JsonDiscriminator("authorizing")]
        public record Authorizing : PaymentDetails;

        /// <summary>
        /// Represents a payment that has been authorized by the end user
        /// </summary>
        /// <returns></returns>
        [JsonDiscriminator("authorized")]
        public record Authorized : PaymentDetails;

        /// <summary>
        /// Represents a payment that has been executed
        /// For open loop payments this state is terminate. For closed-loop payments, wait for Settled.
        /// </summary>
        /// <param name="ExecutedAt">The date and time the payment executed</param>
        /// <param name="SourceOfFunds">Details of the source of funds for the payment</param>
        /// <returns></returns>
        [JsonDiscriminator("executed")]
        public record Executed(DateTime ExecutedAt, SourceOfFundsUnion SourceOfFunds) : PaymentDetails;

        /// <summary>
        /// Represents a payment that has settled
        /// This is a terminate state for closed-loop payments.
        /// </summary>
        /// <param name="ExecutedAt">The date and time the payment executed</param>
        /// <param name="SettledAt">The date and time the payment was settled</param>
        /// <param name="SourceOfFunds">Details of the source of funds for the payment</param>
        /// <returns></returns>
        [JsonDiscriminator("settled")]
        public record Settled(DateTime ExecutedAt, DateTime SettledAt, SourceOfFundsUnion SourceOfFunds) : PaymentDetails;

        /// <summary>
        /// Represents an authorized payment that failed to complete. This is a terminate state.
        /// </summary>
        /// <param name="FailedAt">The date and time the payment failed</param>
        /// <param name="FailureStage">The status the payment was in when it failed</param>
        /// <param name="FailureReason">The reason for failure</param>
        /// <returns></returns>
        [JsonDiscriminator("failed")]
        public record Failed(DateTime FailedAt, string FailureStage, string FailureReason) : PaymentDetails;
    }
}
