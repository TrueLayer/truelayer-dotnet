using System;
using Microsoft.Extensions.Options;
using TrueLayer.Auth;
using TrueLayer.MerchantAccounts;
using TrueLayer.Payments;
using TrueLayer.PaymentsProviders;
using TrueLayer.Payouts;
using TrueLayer.Mandates;

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

    internal class TrueLayerClient : ITrueLayerClient
    {
        // APIs that require specific configuration should be lazily initialised
        private readonly Lazy<IPaymentsApi> _payments;
        private readonly Lazy<IPaymentsProvidersApi> _paymentsProviders;
        private readonly Lazy<IPayoutsApi> _payouts;
        private readonly Lazy<IMerchantAccountsApi> _merchants;
        private readonly Lazy<IMandatesApi> _mandates;

        public TrueLayerClient(IAuthApi auth, Lazy<IPaymentsApi> payments, Lazy<IPaymentsProvidersApi> paymentsProviders, Lazy<IPayoutsApi> payouts, Lazy<IMerchantAccountsApi> merchants, Lazy<IMandatesApi> mandates)
        {
            Auth = auth;
            _payments = payments;
            _paymentsProviders = paymentsProviders;
            _payouts = payouts;
            _merchants = merchants;
            _mandates = mandates;

        }

        public IAuthApi Auth { get; }
        public IPaymentsApi Payments => _payments.Value;
        public IPaymentsProvidersApi PaymentsProviders => _paymentsProviders.Value;
        public IPayoutsApi Payouts => _payouts.Value;
        public IMerchantAccountsApi MerchantAccounts => _merchants.Value;
        public IMandatesApi Mandates => _mandates.Value;
    }
}
