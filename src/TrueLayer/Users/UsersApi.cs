using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Users.Model;

namespace TrueLayer.Users
{
    internal class UsersApi : IUsersApi
    {
        private const string ProdUrl = "https://api.truelayer.com/users/";
        private const string SandboxUrl = "https://api.truelayer-sandbox.com/users/";
        private static string[] RequiredScopes = new[] { "paydirect" };

        private readonly IApiClient _apiClient;
        private readonly Uri _baseUri;
        private readonly IAuthApi _auth;

        public UsersApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _auth = auth.NotNull(nameof(auth));

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            _baseUri = options.Payments.Uri is not null
                ? new Uri(options.Payments.Uri, "users/")
                : new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        public async Task<ApiResponse<GetUserExternalAccountsResponse>> GetUserExternalAccounts(string id, CancellationToken cancellationToken = default)
        {
            id.NotNullOrWhiteSpace(nameof(id));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(RequiredScopes), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<GetUserExternalAccountsResponse>(
                new Uri(_baseUri, $"{id}/external-accounts"),
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }
    }
}
