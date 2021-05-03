namespace TrueLayer.Auth.Model
{
    public record AuthTokenResponse
    {
        public string AccessToken { get; init; } = null!;
        public int ExpiresIn { get; init; }
        public string TokenType { get; init; } = null!;
        public string? Scope { get; set; }
    }
}
