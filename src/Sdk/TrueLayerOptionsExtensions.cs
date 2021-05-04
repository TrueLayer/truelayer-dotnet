using TrueLayer;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// TrueLayer SDK extensions for <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    public static class TrueLayerOptionsExtensions
    {
        /// <summary>
        /// Gets the options from the "TrueLayer" configuration section needed to configure the TrueLayer.com SDK for .NET.
        /// </summary>
        /// <param name="configuration">The configuration properties.</param>
        /// <param name="sectionName">The section name from which to bind the configuration.</param>
        /// <returns>The TrueLayer options initialized with values from the provided configuration.</returns>
        public static TrueLayerOptions GetTrueLayerOptions(this IConfiguration configuration, string sectionName = "TrueLayer") =>
            configuration.GetSection(sectionName).Get<TrueLayerOptions>();
    }
}
