using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Caching;

namespace TrueLayer.Auth
{
    internal class AuthApiCacheDecorator : IAuthApi
    {
        private readonly IAuthApi _client;
        private readonly IAuthTokenCache _authTokenCache;
        private readonly TrueLayerOptions _options;
        private readonly TimeSpan _minTimeToRenew = TimeSpan.FromMinutes(1);
        private const string KeyPrefix = "tl-auth-token";

        public AuthApiCacheDecorator(IAuthApi client, IAuthTokenCache authTokenCache, TrueLayerOptions options)
        {
            _client = client;
            _authTokenCache = authTokenCache;
            _options = options;
        }

        public async ValueTask<ApiResponse<GetAuthTokenResponse>> GetAuthToken(
            GetAuthTokenRequest authTokenRequest,
            CancellationToken cancellationToken = default)
        {
            var key = $"{KeyPrefix}:{_options.ClientId}:{authTokenRequest.Scope}";
            if (_authTokenCache.TryGetValue(key, out ApiResponse<GetAuthTokenResponse>? cachedResponse))
            {
                return cachedResponse!;
            }

            var authTokenResponse = await _client.GetAuthToken(authTokenRequest, cancellationToken);

            if (authTokenResponse.IsSuccessful && authTokenResponse.Data is not null)
            {
                var expireIn = TimeSpan.FromSeconds(authTokenResponse.Data.ExpiresIn);

                var expiry = expireIn > _minTimeToRenew
                    ? expireIn - _minTimeToRenew
                    : expireIn;

                _authTokenCache.Set(key, authTokenResponse, expiry);
            }

            return authTokenResponse;
        }
    }
}
