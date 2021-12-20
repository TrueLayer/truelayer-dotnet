using System;
using TrueLayer.Serialization;

namespace TrueLayer.Payouts.Model
{
    /// <summary>
    /// Get Payout Response Types
    /// </summary>
    public static class GetPayoutsResponse
    {
        /// <summary>
        /// Base class containing common properties for all payout states
        /// </summary>
        /// <value></value>
        public record PayoutDetails
        {
            /// <summary>
            /// Gets the unique identifier of the payout
            /// </summary>
            public string Id { get; init; } = null!;

            /// <summary>
            /// Gets the unique identifier of the merchant account
            /// </summary>
            public string MerchantAccountId { get; init; } = null!;

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
            /// Gets the status of the payout
            /// </summary>
            public string Status { get; init; } = null!;

            /// <summary>
            /// Gets the data and time the payout was created
            /// </summary>
            /// <value></value>
            public DateTime CreatedAt { get; init; }
        }

        /// <summary>
        /// Represents a payout that is pending
        /// </summary>
        [JsonDiscriminator("pending")]
        public record Pending : PayoutDetails;

        /// <summary>
        /// Represents a payout that has been authorized by the end user
        /// </summary>
        /// <returns></returns>
        [JsonDiscriminator("authorized")]
        public record Authorized : PayoutDetails;

        /// <summary>
        /// Represents a payout that has succeeded
        /// For open loop payouts this state is terminate. For closed-loop payouts, wait for Settled.
        /// </summary>
        /// <param name="SucceededAt">The date and time the payout succeeded</param>
        /// <returns></returns>
        [JsonDiscriminator("succeeded")]
        public record Succeeded(DateTime SucceededAt) : PayoutDetails;

        /// <summary>
        /// Represents an authorized payout that failed to complete. This is a terminate state.
        /// </summary>
        /// <param name="FailedAt">The date and time the payout failed</param>
        /// <param name="FailureReason">The reason for failure</param>
        /// <returns></returns>
        [JsonDiscriminator("failed")]
        public record Failed(DateTime FailedAt, string FailureReason) : PayoutDetails;
    }
}
