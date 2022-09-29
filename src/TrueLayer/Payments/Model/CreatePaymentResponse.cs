using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Create Payment Response Types
    /// </summary>
    public static class CreatePaymentResponse
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
            /// Gets the token used to complete the payment via a front-end channel
            /// </summary>
            public string ResourceToken { get; init; } = null!;

            /// <summary>
            /// Gets the end user details
            /// </summary>
            public PaymentUserResponse User { get; init; } = null!;

            /// <summary>
            /// Gets the status of the payment
            /// </summary>
            public string Status { get; init; } = null!;
        }

        /// <summary>
        /// Represents a payment that requires further authorization
        /// </summary>
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired : PaymentDetails;

        /// <summary>
        /// Represents a payment that has been authorized by the end user
        /// </summary>
        [JsonDiscriminator("authorized")]
        public record Authorized : PaymentDetails;

        /// <summary>
        /// Represents a payment that failed to complete. This is a terminal state.
        /// </summary>
        /// <param name="FailureStage">The status the payment was in when it failed</param>
        /// <param name="FailureReason">The reason for failure</param>
        [JsonDiscriminator("failed")]
        public record Failed(string FailureStage, string FailureReason) : PaymentDetails;
    }
}
