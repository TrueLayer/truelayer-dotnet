using System;
using TrueLayer.Auth;

namespace TrueLayer.Caching
{
    internal class NullMemoryCache : IAuthTokenCache
    {
        public bool TryGetValue(string key, out ApiResponse<GetAuthTokenResponse>? value)
        {
            value = null;
            return false;
        }

        public void Set(string key, ApiResponse<GetAuthTokenResponse> value, TimeSpan absoluteExpirationRelativeToNow)
        {
        }

        public void Clear()
        {
        }
    }
}
