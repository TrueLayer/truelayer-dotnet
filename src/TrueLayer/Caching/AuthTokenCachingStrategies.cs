namespace TrueLayer.Caching;

/// <summary>
/// Represents the different strategies for caching auth token
/// </summary>
public enum AuthTokenCachingStrategies
{
    /// <summary>
    /// No caching.
    /// A new auth token is generated for each request.
    /// </summary>
    None,

    /// <summary>
    /// Add a InMemory cache to store the auth token.
    /// Recommended option as it improves latency and availability
    /// </summary>
    InMemory,

    /// <summary>
    /// Add a custom caching to store the auth token.
    /// To use this strategy it is required to implement <see cref="IAuthTokenCache"/> interface
    /// and add the custom caching class as a singleton in DI
    /// </summary>
    Custom
}