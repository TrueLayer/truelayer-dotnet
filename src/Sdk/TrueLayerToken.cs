using System;

namespace TrueLayer
{
    public class TrueLayerToken
    {
        /// <summary>
        /// Stores the newly created auth payment token for later use.
        /// </summary>
        /// <param name="value">The serialized token.</param>
        /// <param name="expiresIn">Expiration in seconds.</param>
        /// <param name="scope">Token scope.</param>
        /// <param name="type">Token type.</param>
        public TrueLayerToken(string value, int expiresIn, string scope, string type)
        {
            Value = value;
            ExpiresIn = expiresIn;
            Scope = scope;
            TokenType = type;
            
            _expiration = DateTime.UtcNow.AddSeconds(expiresIn);
        }
        
        /// <summary>
        /// The serialized token.
        /// </summary>
        public string Value { get; }
        
        /// <summary>
        /// Expiration in seconds.
        /// </summary>
        public int ExpiresIn { get; }
        
        /// <summary>
        /// Token scope.
        /// </summary>
        public string Scope { get; }
        
        /// <summary>
        /// Token type.
        /// </summary>
        public string TokenType { get; }

        private readonly DateTime _expiration;

        /// <summary>
        /// Validates the token. Validation only takes expiration into account at the moment.
        /// </summary>
        /// <returns>A boolean indicating whether the token is valid or not.</returns>
        public bool IsValid() => DateTime.UtcNow < _expiration;
    }
}
