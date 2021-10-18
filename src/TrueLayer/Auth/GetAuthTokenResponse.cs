namespace TrueLayer.Auth
{
    public record GetAuthTokenResponse(string AccessToken, int ExpiresIn, string TokenType, string? Scope);
}
