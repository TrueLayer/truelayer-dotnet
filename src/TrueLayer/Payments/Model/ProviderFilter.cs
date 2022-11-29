namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Filter options for providers
    /// </summary>
    public record ProviderFilter
    {
        /// <summary>
        /// Gets or inits the an array of ISO 3166-1 alpha-2 country codes used to filter providers e.g. GB.
        /// </summary>
        public string[]? Countries { get; init; }

        /// <summary>
        /// The lowest stability release stage of a provider that should be returned.
        /// See <see cref="ReleaseChannels"/>.
        /// </summary>
        /// <example>ReleaseChannels.PrivateBeta</example>
        public string? ReleaseChannel { get; init; }

        /// <summary>
        /// Gets or inits the customer segments catered to by a provider that should be returned.
        /// See <see cref="CustomerSegments"/>.
        /// </summary>
        /// <example>CustomerSegments.Business</example>
        public string? CustomerSegments { get; init; }

        /// <summary>
        /// Gets or inits the identifiers of the specific providers that should be returned.
        /// </summary>
        public string[]? ProviderIds { get; init; }

        /// <summary>
        /// Gets or inits the filters used to exclude specific providers
        /// </summary>
        public ExcludesFilter? Excludes { get; init; }

        public record ExcludesFilter
        {
            /// <summary>
            /// Gets or inits the identifiers of the specific providers that should be excluded
            /// </summary>
            /// <value></value>
            public string[]? ProviderIds { get; init; }
        }
    }
}
