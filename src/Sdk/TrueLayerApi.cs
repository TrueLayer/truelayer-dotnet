using System;
using TrueLayer.Auth;
using TrueLayer.PayDirect.Model;
using TrueLayer.Payments;

namespace TrueLayer
{
    internal class TrueLayerApi : ITrueLayerApi
    {
        private readonly Lazy<IAuthClient> _auth;
        private readonly Lazy<IPaymentsClient> _payments;
        private readonly Lazy<IPayDirectClient> _payDirect; 
        
        /// <summary>
        /// Creates a new <see cref="TrueLayerApi"/> instance and initializes each underlying API client.
        /// </summary>
        /// <param name="apiClient">The API client used to send API requests and handle responses.</param>
        /// <param name="options">A options object containing authentication and API specific information.</param>
        public TrueLayerApi(IApiClient apiClient, TrueLayerOptions options)
        {
            apiClient.NotNull(nameof(apiClient));
            options.NotNull(nameof(options));

            _auth = new(() => new AuthClient(apiClient, options));
            _payments = new(() => new PaymentsClient(apiClient, options, Auth));
            _payDirect = new(() => new PayDirectClient(apiClient, Auth, options));
        }

        public IAuthClient Auth => _auth.Value;
        public IPaymentsClient Payments => _payments.Value;
        public IPayDirectClient PayDirect => _payDirect.Value;
    }
}
