using System;
using TrueLayer.Auth;
using TrueLayer.MerchantAccounts;
using TrueLayer.Payments;
using TrueLayer.PaymentsProviders;
using TrueLayer.Payouts;
using TrueLayer.Mandates;

namespace TrueLayer
{
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
