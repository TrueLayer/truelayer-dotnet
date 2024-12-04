using System;
using TrueLayer.Auth;

namespace TrueLayer;

public interface IAuthTokenCache
{
    bool TryGetValue(string key, out ApiResponse<GetAuthTokenResponse>? value);
    void Set(string key, ApiResponse<GetAuthTokenResponse> value, TimeSpan absoluteExpirationRelativeToNow);
}
