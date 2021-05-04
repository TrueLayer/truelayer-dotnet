using TrueLayer.Auth;
using TrueLayer.Payments;

namespace TrueLayer
{
    internal class TrueLayerApi : ITrueLayerApi
    {
        /// <summary>
        /// Creates a new <see cref="TrueLayerApi"/> instance and initializes each underlying API client.
        /// </summary>
        /// <param name="apiClient">The API client used to send API requests and handle responses.</param>
        /// <param name="options">A options object containing authentication and API specific information.</param>
        public TrueLayerApi(IApiClient apiClient, TrueLayerOptions options)
        {
            Auth = new AuthClient(apiClient, options);
            Payments = new PaymentsClient(apiClient, options);
        }
        
        public IAuthClient Auth { get; }
        public IPaymentsClient Payments { get; }
    }
}
