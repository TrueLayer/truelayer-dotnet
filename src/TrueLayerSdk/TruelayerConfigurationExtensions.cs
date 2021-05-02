using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Truelayer SDK extensions for <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    public static class TruelayerConfigurationExtensions
    {
        /// <summary>
        /// Gets the options from the "Truelayer" configuration section needed to configure the Truelayer.com SDK for .NET.
        /// </summary>
        /// <param name="configuration">The configuration properties.</param>
        /// <param name="sectionName">The section name from which to bind the configuration.</param>
        /// <returns>The Truelayer options initialized with values from the provided configuration.</returns>
        public static TruelayerOptions GetTruelayerOptions(this IConfiguration configuration, string sectionName = "TrueLayer") =>
            configuration.GetSection(sectionName).Get<TruelayerOptions>();

        /// <summary>
        /// Helper method to try extract a value from dictionary without throwing.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key we're looking for.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TResult">Type of the result we expect.</typeparam>
        /// <returns></returns>
        public static TResult GetValueSafe<TKey, TResult>(this IReadOnlyDictionary<TKey, TResult> dict, TKey key)
        {
            if (dict is null) return default;
            return dict.TryGetValue(key, out var uri) ? uri : default;
        }
    }
}
