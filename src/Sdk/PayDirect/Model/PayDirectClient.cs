using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Auth.Model;

namespace TrueLayer.PayDirect.Model
{
    public class PayDirectClient : IPayDirectClient
    {
        internal const string ProdUrl = "https://paydirect.truelayer.com/v1/";
        internal const string SandboxUrl = "https://paydirect.truelayer-sandbox.com/v1/";
        internal static string[] RequiredScopes = new[] { "paydirect" };

        private readonly IApiClient _apiClient;
        private readonly IAuthClient _authClient;
        private readonly PayDirectOptions _options;
        private readonly Uri _baseUri;

        internal PayDirectClient(IApiClient apiClient, IAuthClient authClient, TrueLayerOptions options)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _authClient = authClient ?? throw new ArgumentNullException(nameof(authClient));
            _options = options?.PayDirect ?? throw new ArgumentNullException(nameof(options));
            _baseUri = options.PayDirect?.Uri ??
                       new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);

            _options.Validate();
        }

        public async Task<IEnumerable<AccountBalance>> GetAccountBalances(CancellationToken cancellationToken = default)
        {
            const string path = "balances";

            AuthTokenResponse authToken = await _authClient.GetOAuthToken(RequiredScopes, cancellationToken);

            return (await _apiClient.GetAsync<ApiResultCollection<AccountBalance>>(GetRequestUri(path), authToken.AccessToken, cancellationToken)).Results;
        }

        public async Task<InitiateDepositResponse> InitiateDeposit(InitiateDepositRequest request, CancellationToken cancellationToken = default)
        {
            request.NotNull(nameof(request));

            const string path = "users/deposits";

            AuthTokenResponse authToken = await _authClient.GetOAuthToken(RequiredScopes, cancellationToken);
            return await _apiClient.PostAsync<ApiResult<InitiateDepositResponse>>(GetRequestUri(path), request, authToken.AccessToken, _options.SigningKey, cancellationToken);
        }

        private Uri GetRequestUri(string path) => new(_baseUri, path);
    }
}
