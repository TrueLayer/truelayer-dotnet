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
        
        public IAuthClient Auth { get; }
        public IDataClient Data { get; }
        public IPaymentsClient Payments { get; }
    }
}
