using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payouts.Model.Beneficiary;


namespace TrueLayer.Payouts.Model
{
    using BeneficiaryUnion = OneOf<PaymentSource, ExternalAccount, BusinessAccount>;

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
            /// Gets the beneficiary details
            /// </summary>
            public BeneficiaryUnion Beneficiary { get; init; }

            /// <summary>
            /// Gets the status of the payout
            /// </summary>
            public string Status { get; init; } = null!;

            /// <summary>
            /// Gets the data and time the payout was created
            /// </summary>
            /// <value></value>
            public DateTime CreatedAt { get; init; }

            /// <summary>
            /// Gets the scheme id
            /// </summary>
            public string? SchemeId { get; init; } = null;

            /// <summary>
            /// Gets metadata of the payout
            /// </summary>
            public Dictionary<string, string>? Metadata { get; init; }

            /// <summary>
            /// Gets the sub-merchants details
            /// </summary>
            [JsonPropertyName("sub_merchants")]
            public PayoutSubMerchants? SubMerchants { get; init; }
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
        /// Represents a payout that has been executed.
        /// </summary>
        /// <param name="ExecutedAt">The date and time the payout got executed</param>
        /// <returns></returns>
        [JsonDiscriminator("executed")]
        public record Executed(DateTime ExecutedAt) : PayoutDetails;

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
