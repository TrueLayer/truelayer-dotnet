using System;
using Microsoft.Extensions.Options;
using TrueLayer.Auth;
using TrueLayer.Mandates;
using TrueLayer.MerchantAccounts;
using TrueLayer.Payments;
using TrueLayer.PaymentsProviders;
using TrueLayer.Payouts;

namespace TrueLayer
{
    internal class TrueLayerClientFactory
    {
        private readonly IApiClient _apiClient;
        private readonly IOptions<TrueLayerOptions> _options;
        private readonly IAuthTokenCache _authTokenCache;

        public TrueLayerClientFactory(IApiClient apiClient, IOptions<TrueLayerOptions> options, IAuthTokenCache authTokenCache)
        {
            apiClient.NotNull(nameof(apiClient));
            options.NotNull(nameof(options));

            _apiClient = apiClient;
            _options = options;
            _authTokenCache = authTokenCache;
        }

        public ITrueLayerClient Create()
        {
            var auth = new AuthApi(_apiClient, _options.Value);

            return new TrueLayerClient(
                auth,
                new Lazy<IPaymentsApi>(() => new PaymentsApi(_apiClient, auth, _options.Value)),
                new Lazy<IPaymentsProvidersApi>(() => new PaymentsProvidersApi(_apiClient, auth, _options.Value)),
                new Lazy<IPayoutsApi>(() => new PayoutsApi(_apiClient, auth, _options.Value)),
                new Lazy<IMerchantAccountsApi>(() => new MerchantAccountsApi(_apiClient, auth, _options.Value)),
                new Lazy<IMandatesApi>(() => new MandatesApi(_apiClient, auth, _options.Value)));
        }

        public ITrueLayerClient CreateWithCache()
        {
            var auth = new AuthApiCacheDecorator(new AuthApi(_apiClient, _options.Value), _authTokenCache);

            return new TrueLayerClient(
                auth,
                new Lazy<IPaymentsApi>(() => new PaymentsApi(_apiClient, auth, _options.Value)),
                new Lazy<IPaymentsProvidersApi>(() => new PaymentsProvidersApi(_apiClient, auth, _options.Value)),
                new Lazy<IPayoutsApi>(() => new PayoutsApi(_apiClient, auth, _options.Value)),
                new Lazy<IMerchantAccountsApi>(() => new MerchantAccountsApi(_apiClient, auth, _options.Value)),
                new Lazy<IMandatesApi>(() => new MandatesApi(_apiClient, auth, _options.Value)));
        }
    }
}
