using System;

namespace TrueLayerSdk
{
    public class TruelayerConfiguration
    {
        // PROD
        public const string AuthProductionUri = "https://auth.truelayer.com/";
        public const string DataProductionUri = "https://api.truelayer.com/";
        public const string PaymentsProductionUri = "https://pay-api.truelayer.com/";

        // SANDBOX
        public const string AuthSandboxUri = "https://auth.truelayer-sandbox.com/";
        public const string DataSandboxUri = "https://api.truelayer-sandbox.com/";
        public const string PaymentsSandboxUri = "https://pay-api.truelayer-sandbox.com/";
        
        /// <summary>
        /// Creates a new <see cref="TruelayerConfiguration"/> instance, explicitly setting the API's base URI. 
        /// </summary>
        /// <param name="clientId">Your client id obtained from the TrueLayer Console.</param>
        /// <param name="clientSecret">Your secret key obtained from the TrueLayer Console.</param>
        /// <param name="useSandbox">Whether to connect to the Truelayer Sandbox. False indicates the live environment should be used.</param>
        public TruelayerConfiguration(string clientId, string clientSecret, bool useSandbox)
        {
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException($"Your client id is required", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException($"Your API secret key is required", nameof(clientSecret));

            ClientId = clientId;
            ClientSecret = clientSecret;
            AuthUri = useSandbox ? AuthSandboxUri : AuthProductionUri;
            DataUri = useSandbox ? DataSandboxUri : DataProductionUri;
            PaymentsUri = useSandbox ? PaymentsSandboxUri : PaymentsProductionUri;
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
        public string AuthUri { get; }
        
        /// <summary>
        /// Gets the Uri of the Truelayer Data API to connect to.
        /// </summary>
        public string DataUri { get; }
        
        /// <summary>
        /// Gets the Uri of the Truelayer Payments API to connect to.
        /// </summary>
        public string PaymentsUri { get; }
    }
}
