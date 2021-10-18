using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer.Auth
{
    internal class AuthApi : IAuthApi
    {
        internal const string ProdUrl = "https://auth.truelayer.com/";
        internal const string SandboxUrl = "https://auth.truelayer-sandbox.com/";

        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;

        public AuthApi(IApiClient apiClient, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));

            _baseUri = options.Auth?.Uri ??
                      new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        public Task<ApiResponse<GetAuthTokenResponse>> GetAuthToken(GetAuthTokenRequest authTokenRequest, CancellationToken cancellationToken = default)
        {
            authTokenRequest.NotNull(nameof(authTokenRequest));

            var values = new List<KeyValuePair<string?, string?>>
            {
                new ("grant_type", "client_credentials"),
                new ("client_id", _options.ClientId),
                new ("client_secret", _options.ClientSecret),
            };

            if (authTokenRequest.IsScoped)
            {
                values.Add(new("scope", authTokenRequest.Scope));
            }

            return _apiClient.PostAsync<GetAuthTokenResponse>(
                new Uri(_baseUri, "connect/token"), new FormUrlEncodedContent(values), null, cancellationToken);
        }
    }
}
