namespace TrueLayer.Auth
{
    /// <summary>
    /// Represents a successful OAuth Access Token response
    /// </summary>
    /// <param name="AccessToken">The OAuth Access Token (JWT)</param>
    /// <param name="ExpiresIn">The token expiry time in seconds</param>
    /// <param name="TokenType">The type of token</param>
    /// <param name="Scope">The token scopes</param>>
    public record GetAuthTokenResponse(string AccessToken, int ExpiresIn, string TokenType, string? Scope);
}
