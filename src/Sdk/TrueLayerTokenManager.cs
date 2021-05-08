namespace TrueLayer
{
    public class TrueLayerTokenManager
    {
        /// <summary>
        /// The token use for payments authentication.
        /// </summary>
        public TrueLayerToken PaymentToken { get; private set; }

        public TrueLayerTokenManager()
        {
            PaymentToken = new TrueLayerToken("", 0, "", "");
        }
        
        /// <summary>
        /// Stores the newly created auth payment token for later use.
        /// </summary>
        /// <param name="payload">The serialized token.</param>
        /// <param name="expiresIn">Expiration in seconds.</param>
        /// <param name="scope">Token scope.</param>
        /// <param name="type">Token type.</param>
        public void SetPaymentToken(string payload, int expiresIn, string scope, string type)
        {
            PaymentToken = new TrueLayerToken(payload, expiresIn, scope, type);
        }
    }
}
