using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Extensions;

namespace TrueLayer.Auth
{
    internal class AuthApi : IAuthApi
    {
        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;

        public AuthApi(IApiClient apiClient, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));
            _baseUri = options.GetAuthBaseUri();
        }

        /// <inheritdoc />
        public async ValueTask<ApiResponse<GetAuthTokenResponse>> GetAuthToken(GetAuthTokenRequest authTokenRequest, CancellationToken cancellationToken = default)
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

            return await _apiClient.PostAsync<GetAuthTokenResponse>(
                _baseUri.Append(AuthEndpoints.Token), new FormUrlEncodedContent(values), null, cancellationToken);
        }
    }
}
