using System.IO;

namespace TrueLayer
{
    /// <summary>
    /// ES512 signing key used to sign API requests
    /// </summary>
    public class SigningKey
    {
        /// <summary>
        /// Gets the ES512 PEM certificate contents
        /// </summary>
        public string Certificate { get; set; } = null!;
        
        /// <summary>
        /// Gets the TrueLayer Key identifier available from the Console
        /// </summary>
        public string KeyId { get; set; } = null!;
    } 
}
