using System;
using Microsoft.Extensions.Options;
using TrueLayer.Auth;
using TrueLayer.Payments;
using TrueLayer.MerchantAccounts;
using TrueLayer.PaymentsProviders;
using TrueLayer.Payouts;

namespace TrueLayer
{
    using TrueLayer.Mandates;

    internal class TrueLayerClient : ITrueLayerClient
    {
        // APIs that require specific configuration should be lazily initialised
        private readonly Lazy<IPaymentsApi> _payments;
        private readonly Lazy<IPaymentsProvidersApi> _paymentsProviders;
        private readonly Lazy<IPayoutsApi> _payouts;
        private readonly Lazy<IMerchantAccountsApi> _merchants;
        private readonly Lazy<IMandatesApi> _mandates;

        public TrueLayerClient(IApiClient apiClient, IOptions<TrueLayerOptions> options)
        {
            apiClient.NotNull(nameof(apiClient));
            options.NotNull(nameof(options));

            Auth = new AuthApi(apiClient, options.Value);
            _payments = new(() => new PaymentsApi(apiClient, Auth, options.Value));
            _paymentsProviders = new(() => new PaymentsProvidersApi(apiClient, Auth, options.Value));
            _payouts = new(() => new PayoutsApi(apiClient, Auth, options.Value));
            _merchants = new(() => new MerchantAccountsApi(apiClient, Auth, options.Value));
            _mandates = new(() => new MandatesApi(apiClient, Auth, options.Value));
        }

        public IAuthApi Auth { get; }
        public IPaymentsApi Payments => _payments.Value;
        public IPaymentsProvidersApi PaymentsProviders => _paymentsProviders.Value;
        public IPayoutsApi Payouts => _payouts.Value;
        public IMerchantAccountsApi MerchantAccounts => _merchants.Value;
        public IMandatesApi Mandates => _mandates.Value;
    }
}
