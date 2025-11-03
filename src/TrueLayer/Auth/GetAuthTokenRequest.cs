namespace TrueLayer.Auth;

/// <summary>
/// Represents the request for an OAuth Access Token
/// </summary>
public class GetAuthTokenRequest
{
    /// <summary>
    /// Creates a new <see cref="GetAuthTokenRequest"/> instance
    /// </summary>
    /// <param name="scopes">An optional array of OAuth scopes otherwise all available scopes will be activated</param>        
    public GetAuthTokenRequest(params string[]? scopes)
    {
        if (scopes is { Length: > 0 })
        {
            Scope = string.Join(' ', scopes);
            IsScoped = true;
        }
    }

    /// <summary>
    /// Gets the OAuth scope value
    /// </summary>
    public string? Scope { get; }

    /// <summary>
    /// Gets whether the access token request is scoped
    /// </summary>
    /// <value>True if the request is scoped, otherwise False</value>
    public bool IsScoped { get; }
}