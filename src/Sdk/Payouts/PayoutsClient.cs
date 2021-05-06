using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Auth.Model;
using TrueLayer.Payouts.Model;
using TrueLayer.Serialization;

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
        private readonly PayoutsOptions _options;
        private readonly Uri _baseUri;

        public PayoutsClient(IApiClient apiClient, IAuthClient authClient, TrueLayerOptions options)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _authClient = authClient ?? throw new ArgumentNullException(nameof(authClient));
            _options = options?.Payouts ?? throw new ArgumentNullException(nameof(options));
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

        public async Task InitiatePayout(InitiatePayoutRequest request, CancellationToken cancellationToken = default)
        {
            request.NotNull(nameof(request));
            
            const string path = "payouts";
            
            AuthTokenResponse authToken = await _authClient.GetOAuthToken(RequiredScopes, cancellationToken);
            _ = await _apiClient.PostAsync<Empty>(GetRequestUri(path), request, authToken.AccessToken, _options.SigningKey, cancellationToken);
        }

        public async Task ValidateSigningKey(CancellationToken cancellationToken = default)
        {
            const string path = "test";

            var request = new {
                nonce = Guid.NewGuid().ToString()
            };

            AuthTokenResponse authToken = await _authClient.GetOAuthToken(RequiredScopes, cancellationToken);
            _ = await _apiClient.PostAsync<Empty>(GetRequestUri(path), request, authToken.AccessToken, _options.SigningKey, cancellationToken);
        }

        private Uri GetRequestUri(string path) => new(_baseUri, path);
    }
}
