using System;
using OneOf;
using TrueLayer.Serialization;
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
        /// <param name="AuthorizedAt">The date and time the payment was authorized</param>
        /// <returns></returns>
        [JsonDiscriminator("authorized")]
        public record Authorized(DateTime AuthorizedAt) : PaymentDetails;

        /// <summary>
        /// Represents a payment that failed to be authorized
        /// </summary>
        /// <param name="FailedAt">The date and time the authorization failed</param>
        /// <param name="FailureReason">The reason for failure</param>
        /// <returns></returns>
        [JsonDiscriminator("authorization_failed")]
        public record AuthorizationFailed(DateTime FailedAt, string FailureReason) : PaymentDetails;

        /// <summary>
        /// Represents a payment that has succeeded
        /// For open loop payments this state is terminate. For closed-loop payments, wait for Settled.
        /// </summary>
        /// <param name="AuthorizedAt">The date and time the payment was authorized</param>
        /// <param name="SucceededAt">The date and time the payment succeeded</param>
        /// <param name="SourceOfFunds">Details of the source of funds for the payment</param>
        /// <returns></returns>
        [JsonDiscriminator("successful")]
        public record Successful(DateTime AuthorizedAt, DateTime SucceededAt, SourceOfFundsUnion SourceOfFunds) : PaymentDetails;

        /// <summary>
        /// Represents a payment that has settled
        /// This is a terminate state for closed-loop payments.
        /// </summary>
        /// <param name="AuthorizedAt">The date and time the payment was authorized</param>
        /// <param name="SucceededAt">The date and time the payment succeeded</param>
        /// <param name="SettledAt">The date and time the payment was settled</param>
        /// <param name="SourceOfFunds">Details of the source of funds for the payment</param>
        /// <returns></returns>
        [JsonDiscriminator("settled")]
        public record Settled(DateTime AuthorizedAt, DateTime SucceededAt, DateTime SettledAt, SourceOfFundsUnion SourceOfFunds) : PaymentDetails;

        /// <summary>
        /// Represents an authorized payment that failed to complete. This is a terminate state.
        /// </summary>
        /// <param name="AuthorizedAt">The date and time the payment was authorized</param>
        /// <param name="FailedAt">The date and time the payment failed</param>
        /// <param name="FailureReason">The reason for failure</param>
        /// <returns></returns>
        [JsonDiscriminator("failed")]
        public record Failed(DateTime AuthorizedAt, DateTime FailedAt, string FailureReason) : PaymentDetails;
    }
}
