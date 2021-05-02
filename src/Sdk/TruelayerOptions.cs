using System;
using System.Collections.Generic;
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
        public string ClientId { get; init; }
        
        /// <summary>
        /// Gets or sets your Truelayer client secret.
        /// </summary>
        public string ClientSecret { get; init; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to connect to the Truelayer Sandbox. 
        /// </summary>
        public bool? UseSandbox { get; init; }
        
        /// <summary>
        /// Creates a <see cref="TrueLayerSdk.TruelayerConfiguration"/> needed to configure the SDK.
        /// </summary>
        /// <returns>The initializes configuration.</returns>
        public TruelayerConfiguration CreateConfiguration()
        {
            Dictionary<Platform, Uri> uris = new();
            if (!string.IsNullOrWhiteSpace(AuthUri))
            {
                uris.Add(Platform.Auth, new Uri(AuthUri));
            }
            if (!string.IsNullOrWhiteSpace(DataUri))
            {
                uris.Add(Platform.Data, new Uri(DataUri));
            }
            if (!string.IsNullOrWhiteSpace(PaymentsUri))
            {
                uris.Add(Platform.Payment, new Uri(PaymentsUri));
            }
            return new(ClientId, ClientSecret, UseSandbox ?? true, uris);
        }

        /// <summary>
        /// Override for the authentication URI.
        /// </summary>
        public string AuthUri { get; init; }
        
        /// <summary>
        /// Override for the data uri.
        /// </summary>
        public string DataUri { get; init; }
        
        /// <summary>
        /// Override for the payments uri.
        /// </summary>
        public string PaymentsUri { get; init; }
    }
}
