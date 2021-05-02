using TrueLayer.Auth;
using TrueLayer.Payments;

namespace TrueLayer
{
    internal class TrueLayerApi : ITrueLayerApi
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerApi"/> instance and initializes each underlying API client.
        /// </summary>
        /// <param name="apiClient">The API client used to send API requests and handle responses.</param>
        /// <param name="configuration">A configuration object containing authentication and API specific information.</param>
        public TrueLayerApi(IApiClient apiClient, TruelayerConfiguration configuration)
        {
            Auth = new AuthClient(apiClient, configuration);
            Payments = new PaymentsClient(apiClient, configuration);
        }
        
        public IAuthClient Auth { get; }
        public IPaymentsClient Payments { get; }
    }
}
