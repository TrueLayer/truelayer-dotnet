using TrueLayer.Auth;
using TrueLayer.Payments;
using System;

namespace TrueLayer
{
    internal class TrueLayerApi : ITrueLayerApi
    {
        private readonly Lazy<IAuthClient> _auth;
        private readonly Lazy<IPaymentsClient> _payments;

        /// <summary>
        /// Creates a new <see cref="TrueLayerApi"/> instance and initializes each underlying API client.
        /// </summary>
        /// <param name="apiClient">The API client used to send API requests and handle responses.</param>
        /// <param name="options">A options object containing authentication and API specific information.</param>
        public TrueLayerApi(IApiClient apiClient, TrueLayerOptions options)
        {
            _auth = new(() => new AuthClient(apiClient, options));
            _payments = new(() => new PaymentsClient(apiClient, options, Auth));
        }
        
        public IAuthClient Auth => _auth.Value;
        public IPaymentsClient Payments => _payments.Value;
    }
}
