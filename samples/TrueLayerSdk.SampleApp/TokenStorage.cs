using System;

namespace TrueLayerSdk.SampleApp
{
    public class TokenStorage
    {
        public string AccessToken { get; private set; }
        private DateTime TokenExpiration { get; set; }
        
        public void SetToken(string token, int expiresIn)
        {
            AccessToken = token;
            TokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn);
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > TokenExpiration;
        }
    }
}
