using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Retry types
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Standard retry feature
        /// </summary>
        [JsonDiscriminator("standard")]
        public record Standard : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="Standard"/>
            /// </summary>
            /// <param name="for">How long to retry this payment for</param>
            public Standard(string? @for)
            {
                For = @for.NotEmptyOrWhiteSpace(nameof(@for));
            }

            /// <summary>
            /// Gets the retry type
            /// </summary>
            public string Type => "standard";

            /// <summary>
            /// How long to retry this payment for
            /// </summary>
            public string? For { get; init; }
        }

        /// <summary>
        /// Smart retry feature
        /// </summary>
        [JsonDiscriminator("smart")]
        public record Smart : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="Smart"/>
            /// </summary>
            /// <param name="for">How long to retry this payment for</param>
            /// <param name="ensureMinimumBalanceInMinor">The payment is attempted only if the remaining balance in the account is at least this amount</param>
            public Smart(string @for, uint? ensureMinimumBalanceInMinor)
            {
                For = @for.NotNullOrWhiteSpace(nameof(@for));
                EnsureMinimumBalanceInMinor = ensureMinimumBalanceInMinor?.GreaterThan((uint)0, nameof(ensureMinimumBalanceInMinor));
            }

            /// <summary>
            /// Gets the retry type
            /// </summary>
            public string Type => "smart";

            /// <summary>
            /// How long to retry this payment for
            /// </summary>
            public string For { get; init; }

            /// <summary>
            /// The payment is attempted only if the remaining balance in the account is at least this amount
            /// </summary>
            public uint? EnsureMinimumBalanceInMinor { get; init; }
        }
    }
}
