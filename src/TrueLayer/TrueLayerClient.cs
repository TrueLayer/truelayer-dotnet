using System;
using Microsoft.Extensions.Options;
using TrueLayer.Auth;
using TrueLayer.Payments;
using TrueLayer.Merchants;

namespace TrueLayer
{
    internal class TrueLayerClient : ITrueLayerClient
    {
        // APIs that require specific configuration should be lazily initialised
        private readonly Lazy<IPaymentsApi> _payments;
        private readonly Lazy<IMerchantsApi> _merchants;

        public TrueLayerClient(IApiClient apiClient, IOptions<TrueLayerOptions> options)
        {
            apiClient.NotNull(nameof(apiClient));
            options.NotNull(nameof(options));

            Auth = new AuthApi(apiClient, options.Value);
            _payments = new(() => new PaymentsApi(apiClient, Auth, options.Value));
            _merchants = new(() => new MerchantsApi(apiClient, Auth, options.Value));
        }

        public IAuthApi Auth { get; }
        public IPaymentsApi Payments => _payments.Value;
        public IMerchantsApi Merchants => _merchants.Value;
    }
}
