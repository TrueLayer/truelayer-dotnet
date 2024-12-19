using System;
using TrueLayer.Auth;

namespace TrueLayer.Caching;

/// <summary>
/// IAuthTokenCache can be used to provide custom implementation of Auth token caching
/// </summary>
public interface IAuthTokenCache
{
    /// <summary>
    /// Try to get the GetAuthTokenResponse associated with the given key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">The value associated with the given key.</param>
    /// <returns><c>true</c> if the key was found. <c>false</c> otherwise.</returns>
    bool TryGetValue(
        string key,
        out ApiResponse<GetAuthTokenResponse>? value);

    /// <summary>
    /// Sets a cache entry with the given key and GetAuthTokenResponse that will expire in the given duration from now.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value to associate with the key.</param>
    /// <param name="absoluteExpirationRelativeToNow">The duration from now after which the cache entry will expire.</param>
    /// <returns>The value that was set.</returns>
    void Set(
        string key,
        ApiResponse<GetAuthTokenResponse> value,
        TimeSpan absoluteExpirationRelativeToNow);

    /// <summary>
    /// Removes all entries in the cache.
    /// </summary>
    void Clear();
}
