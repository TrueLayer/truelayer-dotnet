using System;
using System.Collections.Generic;
using TrueLayer.Auth;

namespace TrueLayer.Tests.Mocks;

public class InMemoryAuthTokenCacheMock : IAuthTokenCache
{
    private readonly Dictionary<string, ApiResponse<GetAuthTokenResponse>?> _dictionary = new();

    public bool TryGetValue(string key, out ApiResponse<GetAuthTokenResponse>? value)
    {
       return _dictionary.TryGetValue(key, out value);
    }

    public void Set(string key, ApiResponse<GetAuthTokenResponse> value, TimeSpan absoluteExpirationRelativeToNow)
    {
        _dictionary.Add(key, value);
    }

    public void Remove(string key)
    {
        throw new NotImplementedException();
    }

    public bool IsEmpty => _dictionary.Count == 0;
}
