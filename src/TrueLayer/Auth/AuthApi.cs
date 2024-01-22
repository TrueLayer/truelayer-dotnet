using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Common;
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

            var baseUri = (options.UseSandbox ?? true)
                ? TrueLayerBaseUris.SandboxAuthBaseUri
                : TrueLayerBaseUris.ProdAuthBaseUri;

            _baseUri = options.Auth?.Uri ?? baseUri;
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
                _baseUri.Append("connect/token"), new FormUrlEncodedContent(values), null, cancellationToken);
        }
    }
}
