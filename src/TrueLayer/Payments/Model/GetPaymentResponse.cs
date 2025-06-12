using System;
using System.Collections.Generic;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.PaymentMethod;

namespace TrueLayer.Payments.Model
{
    using PaymentMethodUnion = OneOf<BankTransfer, Mandate>;

    /// <summary>
    /// Get Payment Response Types
    /// </summary>
    public static class GetPaymentResponse
    {
        /// <summary>
        /// Base class containing common properties for all payment states
        /// </summary>
        /// <value></value>
        public abstract record PaymentDetails
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
            /// Gets the payment method details
            /// </summary>
            public PaymentMethodUnion PaymentMethod { get; init; }

            /// <summary>
            /// Gets the end user details
            /// </summary>
            public PaymentUser User { get; init; } = null!;

            /// <summary>
            /// Gets the metadata added to the payment.
            /// </summary>
            public Dictionary<string, string>? Metadata { get; init; } = null;

            /// <summary>
            /// Gets the sub-merchants details
            /// </summary>
            public PaymentSubMerchants? SubMerchants { get; init; } = null;
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
        /// <param name="CreditableAt">The date and time that TrueLayer determined that the payment was ready to be credited</param>
        /// </summary>
        /// <returns></returns>
        [JsonDiscriminator("authorized")]
        public record Authorized(DateTime? CreditableAt) : PaymentDetails;

        /// <summary>
        /// Represents a payment that has been executed
        /// For open loop payments this state is terminal. For closed-loop payments, wait for Settled.
        /// </summary>
        /// <param name="ExecutedAt">The date and time the payment executed</param>
        /// <param name="CreditableAt">The date and time that TrueLayer determined that the payment was ready to be credited</param>
        /// <returns></returns>
        [JsonDiscriminator("executed")]
        public record Executed(DateTime ExecutedAt, DateTime? CreditableAt) : PaymentDetails;

        /// <summary>
        /// Represents a payment that has settled
        /// This is a terminal state for closed-loop payments.
        /// </summary>
        /// <param name="ExecutedAt">The date and time the payment executed</param>
        /// <param name="SettledAt">The date and time the payment was settled</param>
        /// <param name="PaymentSource">Details of the source of funds for the payment</param>
        /// <param name="CreditableAt">The date and time that TrueLayer determined that the payment was ready to be credited</param>
        /// <returns></returns>
        [JsonDiscriminator("settled")]
        public record Settled(DateTime ExecutedAt, DateTime SettledAt, PaymentSource PaymentSource, DateTime? CreditableAt) : PaymentDetails;

        /// <summary>
        /// Represents a payment that failed to complete. This is a terminal state.
        /// </summary>
        /// <param name="FailedAt">The date and time the payment failed</param>
        /// <param name="FailureStage">The status the payment was in when it failed</param>
        /// <param name="FailureReason">The reason for failure</param>
        /// <returns></returns>
        [JsonDiscriminator("failed")]
        public record Failed(DateTime FailedAt, string FailureStage, string FailureReason) : PaymentDetails;


        /// <summary>
        /// Represents a payment that failed to complete due to an error in the authorization flow
        /// </summary>
        /// <param name="FailedAt">The date and time the payment failed</param>
        /// <param name="FailureStage">The status the payment was in when it failed</param>
        /// <param name="FailureReason">The reason for failure</param>
        [JsonDiscriminator("attempt_failed")]
        public record AttemptFailed(DateTime FailedAt, string FailureStage, string FailureReason) : Failed(FailedAt, FailureStage, FailureReason)
        {
            /// <summary>
            /// Gets the details of the authorization flow used for the payment
            /// </summary>
            public AuthorizationFlow.AuthorizationFlow? AuthorizationFlow { get; init; } = null;

            /// <summary>
            /// Gets the details of the source of funds for the payment<
            /// </summary>
            public PaymentSource? PaymentSource { get; init; } = null;
        }
    }
}
