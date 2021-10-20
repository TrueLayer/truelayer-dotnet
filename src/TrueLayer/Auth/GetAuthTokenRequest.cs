namespace TrueLayer.Auth
{
    public class GetAuthTokenRequest
    {
        public GetAuthTokenRequest(params string[]? scopes)
        {
            if (scopes is { Length: > 0 })
            {
                Scope = string.Join(' ', scopes);
                IsScoped = true;
            }
        }

        public string? Scope { get; }
        public bool IsScoped { get; }
    }
}
