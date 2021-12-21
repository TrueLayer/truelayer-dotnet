using System;
using Microsoft.Extensions.Options;
using TrueLayer.Auth;
using TrueLayer.Payments;
using TrueLayer.MerchantAccounts;
using TrueLayer.Users;

namespace TrueLayer
{
    internal class TrueLayerClient : ITrueLayerClient
    {
        // APIs that require specific configuration should be lazily initialised
        private readonly Lazy<IPaymentsApi> _payments;
        private readonly Lazy<IMerchantAccountsApi> _merchants;
        private readonly Lazy<IUsersApi> _users;

        public TrueLayerClient(IApiClient apiClient, IOptions<TrueLayerOptions> options)
        {
            apiClient.NotNull(nameof(apiClient));
            options.NotNull(nameof(options));

            Auth = new AuthApi(apiClient, options.Value);
            _payments = new(() => new PaymentsApi(apiClient, Auth, options.Value));
            _merchants = new(() => new MerchantAccountsApi(apiClient, Auth, options.Value));
            _users = new(() => new UsersApi(apiClient, Auth, options.Value));
        }

        public IAuthApi Auth { get; }
        public IPaymentsApi Payments => _payments.Value;
        public IMerchantAccountsApi MerchantAccounts => _merchants.Value;
        public IUsersApi Users => _users.Value;
    }
}
