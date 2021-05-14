using TrueLayer.Auth;
using TrueLayer.Payments;
using System;

namespace TrueLayer
{
    internal class TrueLayerApi : ITrueLayerApi
    {
        private readonly Lazy<IPaymentsClient> _payments;

        /// <summary>
        /// Creates a new <see cref="TrueLayerApi"/> instance and initializes each underlying API client.
        /// </summary>
        /// <param name="apiClient">The API client used to send API requests and handle responses.</param>
        /// <param name="options">A options object containing authentication and API specific information.</param>
        public TrueLayerApi(IApiClient apiClient, TrueLayerOptions options)
        {
            Auth = new AuthClient(apiClient, options);
            _payments = new(() => new PaymentsClient(apiClient, options, Auth));
        }
        
        public IAuthClient Auth { get; }
        public IPaymentsClient Payments => _payments.Value;
    }
}
