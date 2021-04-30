using System;

namespace TrueLayer
{
    public class TruelayerConfiguration
    {
        // PROD
        private static readonly Uri AuthProductionUri = new ("https://auth.truelayer.com/");
        private static readonly Uri DataProductionUri = new ("https://api.truelayer.com/");
        private static readonly Uri PaymentsProductionUri = new ("https://pay-api.truelayer.com/");

        // SANDBOX
        private static readonly Uri AuthSandboxUri = new ("https://auth.truelayer-sandbox.com/");
        private static readonly Uri DataSandboxUri = new ("https://api.truelayer-sandbox.com/");
        private static readonly Uri PaymentsSandboxUri = new ("https://pay-api.truelayer-sandbox.com/");
        
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
