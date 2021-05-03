namespace TrueLayer.Auth
{
    public class ExchangeCodeRequest
    {
        public string? Code { get; set; }
        public string? RedirectUri { get; set; }
    }
}
