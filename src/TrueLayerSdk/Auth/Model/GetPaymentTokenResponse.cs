namespace TrueLayer.Auth.Model
{
    public class GetPaymentTokenResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}
