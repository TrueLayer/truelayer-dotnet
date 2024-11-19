using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TrueLayer
{
    /// <summary>
    /// ES512 signing key used to sign API requests
    /// </summary>
    public class SigningKey
    {
        private readonly Lazy<ECDsa> _key;

        public SigningKey()
        {
            _key = new Lazy<ECDsa>(() => CreateECDsaKey(PrivateKey));
        }

        /// <summary>
        /// Sets the private key. Should not be shared with anyone outside of your organisation.
        /// </summary>
        public string PrivateKey { get; set; } = null!;

        /// <summary>
        /// Gets the TrueLayer Key identifier available from the Console
        /// </summary>
        public string KeyId { get; set; } = null!;

        internal ECDsa Value => _key.Value;

        private static ECDsa CreateECDsaKey(string privateKey)
        {
            privateKey.NotNullOrWhiteSpace(nameof(privateKey));

            var key = ECDsa.Create();

            key.ImportFromPem(privateKey);

            return key;
        }

        /// <summary>
        /// Reads and decodes the contents of the PEM private key, removing the header/trailer
        /// Required before .NET 5.0
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        private static byte[] ReadPemContents(string privateKey)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(privateKey))
            {
                string? line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("--"))
                        sb.Append(line);
                }
            }

            return Convert.FromBase64String(sb.ToString());
        }
    }
}
