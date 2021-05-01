using TrueLayer;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Defines the options needed to configure the Truelayer.com SDK for .NET that can be initialized
    /// using the Microsoft configuration framework.
    /// </summary>
    public class TruelayerOptions
    {        
        /// <summary>
        /// Gets or sets your Truelayer client id.
        /// </summary>
        public string? ClientId { get; set; }
        
        /// <summary>
        /// Gets or sets your Truelayer client secret.
        /// </summary>
        public string? ClientSecret { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to connect to the Truelayer Sandbox. 
        /// </summary>
        public bool? UseSandbox { get; set; }
        
        /// <summary>
        /// Creates a <see cref="TrueLayerSdk.TruelayerConfiguration"/> needed to configure the SDK.
        /// </summary>
        /// <returns>The initializes configuration.</returns>
        public TruelayerConfiguration CreateConfiguration() => new(ClientId, ClientSecret, UseSandbox ?? true);
    }
}
