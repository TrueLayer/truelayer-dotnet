using TrueLayer.Serialization;

namespace TrueLayer.Payouts.Model
{
    /// <summary>
    /// Create Payout Response Types
    /// </summary>
    public static class CreatePayoutResponse
    {
        /// <summary>
        /// Base class containing common properties for payout creation responses
        /// </summary>
        public record PayoutCreated
        {
            /// <summary>
            /// Gets the unique identifier of the payout
            /// </summary>
            public string Id { get; init; } = null!;
        }

        /// <summary>
        /// Represents a verified payout that requires further authorization (user_determined beneficiary)
        /// </summary>
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired : PayoutCreated
        {
            /// <summary>
            /// Gets the status of the payout (always "authorization_required")
            /// </summary>
            public string Status { get; init; } = null!;

            /// <summary>
            /// Gets the token used to complete the payout verification via a front-end channel
            /// </summary>
            public string ResourceToken { get; init; } = null!;

            /// <summary>
            /// Gets the end user details
            /// </summary>
            public PayoutUserResponse User { get; init; } = null!;
        }

        /// <summary>
        /// Represents a standard payout (external_account, payment_source, or business_account beneficiary)
        /// </summary>
        [DefaultJsonDiscriminator]
        public record Created : PayoutCreated
        {
        }
    }
}
