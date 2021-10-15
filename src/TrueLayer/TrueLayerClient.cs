using System;
using TrueLayer.Auth;
using TrueLayer.Payments;

namespace TrueLayer
{
    internal class TrueLayerClient : ITrueLayerClient
    {
        // APIs that require specific configuration should be lazily initialised
        private readonly Lazy<IPaymentsApi> _payments;

        public TrueLayerClient(IApiClient apiClient, TrueLayerOptions options)
        {
            apiClient.NotNull(nameof(apiClient));
            options.NotNull(nameof(options));

            Auth = new AuthApi(apiClient, options);
            _payments = new(() => new PaymentsApi(apiClient, options));
        }

        public IAuthApi Auth { get; }
        public IPaymentsApi Payments => _payments.Value;
    }
}
