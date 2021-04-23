using TrueLayerSdk.Auth;
using TrueLayerSdk.Data;
using TrueLayerSdk.Payments;

namespace TrueLayerSdk
{
    public class TruelayerApi : ITruelayerApi
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerApi"/> instance and initializes each underlying API client.
        /// </summary>
        /// <param name="apiClient">The API client used to send API requests and handle responses.</param>
        /// <param name="configuration">A configuration object containing authentication and API specific information.</param>
        public TruelayerApi(IApiClient apiClient, TruelayerConfiguration configuration)
        {
            Auth = new AuthClient(apiClient, configuration);
            Data = new DataClient(apiClient, configuration);
            Payments = new PaymentsClient(apiClient, configuration);
        }
        
        /// <summary>
        /// Creates a new <see cref="TruelayerApi"/> instance with default dependencies.
        /// </summary>
        /// <param name="clientId">Your client id obtained from the TrueLayer Console.</param>
        /// <param name="clientSecret">Your secret key obtained from the TrueLayer Console.</param>
        /// <param name="useSandbox">Whether to connect to the Truelayer Sandbox. False indicates the live environment should be used.</param>
        /// <returns>The configured instance.</returns>
        public static TruelayerApi Create(string clientId, string clientSecret, bool useSandbox = true)
        {
            var configuration = new TruelayerConfiguration(clientId, clientSecret, useSandbox);

            var apiClient = new ApiClient(configuration);
            return new TruelayerApi(apiClient, configuration);
        }
        
        public IAuthClient Auth { get; }
        public IDataClient Data { get; }
        public IPaymentsClient Payments { get; }
    }
}
