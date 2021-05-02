using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TrueLayer;

namespace TrueLayerSdk
{
    public class TruelayerConfiguration
    {
        // PROD
        internal static readonly Uri AuthProductionUri = new ("https://auth.truelayer.com/");
        internal static readonly Uri DataProductionUri = new ("https://api.truelayer.com/");
        internal static readonly Uri PaymentsProductionUri = new ("https://pay-api.truelayer.com/");

        // SANDBOX
        internal static readonly Uri AuthSandboxUri = new ("https://auth.truelayer-sandbox.com/");
        internal static readonly Uri DataSandboxUri = new ("https://api.truelayer-sandbox.com/");
        internal static readonly Uri PaymentsSandboxUri = new ("https://pay-api.truelayer-sandbox.com/");
        
        /// <summary>
        /// Creates a new <see cref="TruelayerConfiguration"/> instance, explicitly setting the API's base URI. 
        /// </summary>
        /// <param name="clientId">Your client id obtained from the TrueLayer Console.</param>
        /// <param name="clientSecret">Your secret key obtained from the TrueLayer Console.</param>
        /// <param name="useSandbox">Whether to connect to the Truelayer Sandbox. False indicates the live environment should be used.</param>
        /// <param name="uris">Dictionary containing auth, data and payments uris.</param>
        public TruelayerConfiguration(string clientId, string clientSecret, bool useSandbox, IReadOnlyDictionary<Platform, Uri> uris = null)
        {
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException($"Your client id is required", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException($"Your API secret key is required", nameof(clientSecret));

            ClientId = clientId;
            ClientSecret = clientSecret;
            AuthUri = uris?.GetValueSafe(Platform.Auth) ?? (useSandbox ? AuthSandboxUri : AuthProductionUri);
            DataUri = uris?.GetValueSafe(Platform.Data) ?? (useSandbox ? DataSandboxUri : DataProductionUri);
            PaymentsUri = uris?.GetValueSafe(Platform.Payment) ?? (useSandbox ? PaymentsSandboxUri : PaymentsProductionUri);
        }
        
        /// <summary>
        /// Gets the client id that will be used to authenticate to the Truelayer API.
        /// </summary>
        public string ClientId { get; }
        
        /// <summary>
        /// Gets the secret key that will be used to authenticate to the Truelayer API.
        /// </summary>
        public string ClientSecret { get; }
        
        /// <summary>
        /// Gets the Uri of the Truelayer Authentication API to connect to.
        /// </summary>
        public Uri AuthUri { get; }
        
        /// <summary>
        /// Gets the Uri of the Truelayer Data API to connect to.
        /// </summary>
        public Uri DataUri { get; }
        
        /// <summary>
        /// Gets the Uri of the Truelayer Payments API to connect to.
        /// </summary>
        public Uri PaymentsUri { get; }
    }
}
