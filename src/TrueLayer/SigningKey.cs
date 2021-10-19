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
        private Lazy<ECDsa> _key;

        public SigningKey()
        {
            _key = new Lazy<ECDsa>(() => CreateECDsaKey(Certificate));
        }
        
        /// <summary>
        /// Gets the ES512 PEM certificate contents
        /// </summary>
        public string Certificate { internal get; set; } = null!;

        /// <summary>
        /// Gets the TrueLayer Key identifier available from the Console
        /// </summary>
        public string KeyId { get; set; } = null!;

        internal ECDsa Value => _key.Value;

        private static ECDsa CreateECDsaKey(string certificate)
        {           
            certificate.NotNullOrWhiteSpace(nameof(certificate));
            
            var key = ECDsa.Create();

#if (NET5_0 || NET5_0_OR_GREATER)
            // Ref https://www.scottbrady91.com/C-Sharp/PEM-Loading-in-dotnet-core-and-dotnet
            key.ImportFromPem(certificate);
#else
            byte[] decodedPem = ReadPemContents(certificate);
            key.ImportECPrivateKey(decodedPem, out _);
#endif

            return key;
        }

        /// <summary>
        /// Reads and decodes the contents of the PEM certificate, removing the header/trailer
        /// Required before .NET 5.0
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        private static byte[] ReadPemContents(string certificate)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(certificate))
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
