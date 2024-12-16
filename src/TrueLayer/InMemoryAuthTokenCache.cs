using System;
using Microsoft.Extensions.Caching.Memory;
using TrueLayer.Auth;

namespace TrueLayer
{
    internal class InMemoryAuthTokenCache : IAuthTokenCache
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryAuthTokenCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetValue(string key, out ApiResponse<GetAuthTokenResponse>? value)=>
            _memoryCache.TryGetValue(key, out value);

        public void Set(string key, ApiResponse<GetAuthTokenResponse> value, TimeSpan absoluteExpirationRelativeToNow)=>
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);

        public void Clear()
        {
            var cache = _memoryCache as MemoryCache;
            cache?.Clear();
        }
    }
}
