using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Auth.Model;
using TrueLayer.Payouts.Model;

namespace TrueLayer.Payouts
{
    /// <summary>
    /// Default implementation of <see cref="IPayoutsClient" />
    /// </summary>
    internal class PayoutsClient : IPayoutsClient
    {
        internal const string ProdUrl = "https://payouts.truelayer.com/v1/";
        internal const string SandboxUrl = "https://payouts.truelayer-sandbox.com/v1/";
        internal static string[] RequiredScopes = new[] { "payouts" };

        private readonly IApiClient _apiClient;
        private readonly IAuthClient _authClient;
        private readonly Uri _baseUri;

        public PayoutsClient(IApiClient apiClient, IAuthClient authClient, TrueLayerOptions options)
        {
            _apiClient = apiClient;
            _authClient = authClient;
            _baseUri = options.Payouts?.Uri ??
                       new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        /// <inheritdoc />
        public async Task<QueryResponse<AccountBalance>> GetAccountBalances(CancellationToken cancellationToken = default)
        {
            const string path = "balances";

            AuthTokenResponse authToken = await _authClient.GetOAuthToken(RequiredScopes, cancellationToken);
            return await _apiClient.GetAsync<QueryResponse<AccountBalance>>(GetRequestUri(path), authToken.AccessToken, cancellationToken);
        }

        private Uri GetRequestUri(string path) => new(_baseUri, path);
    }
}
