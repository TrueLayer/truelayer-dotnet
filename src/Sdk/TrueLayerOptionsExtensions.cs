using TrueLayer;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Truelayer SDK extensions for <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    public static class TrueLayerOptionsExtensions
    {
        /// <summary>
        /// Gets the options from the "Truelayer" configuration section needed to configure the Truelayer.com SDK for .NET.
        /// </summary>
        /// <param name="configuration">The configuration properties.</param>
        /// <param name="sectionName">The section name from which to bind the configuration.</param>
        /// <returns>The Truelayer options initialized with values from the provided configuration.</returns>
        public static TruelayerOptions GetTruelayerOptions(this IConfiguration configuration, string sectionName = "TrueLayer") =>
            configuration.GetSection(sectionName).Get<TruelayerOptions>();
    }
}
