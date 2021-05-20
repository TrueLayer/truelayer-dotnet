using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Jose;

namespace TrueLayer
{
    internal static class RequestSigning
    {
        /// <summary>
        /// Generates a Json Web Signature (JWS) using the provided ES512 signing certificate
        /// </summary>
        /// <param name="json">The JSON payload to sign</param>
        /// <param name="signingCertificate">The certificate contents</param>
        /// <param name="keyId">The TrueLayer key ID</param>
        /// <returns></returns>
        public static string SignJson(string json, SigningKey signingKey)
        {
            json.NotNull(nameof(json));
            signingKey.NotNull(nameof(signingKey));
            
            using var key = ECDsa.Create();

            // Ref https://www.scottbrady91.com/C-Sharp/PEM-Loading-in-dotnet-core-and-dotnet
#if !NET5_0
            byte[] decodedPem = ReadPemContents(signingKey.SigningCertificate);
            key.ImportECPrivateKey(decodedPem, out _);
#else
            key.ImportFromPem(signingKey.Certificate);
#endif

            var headers = new Dictionary<string, object> { { "alg", "ES512" }, { "kid", signingKey.KeyId } };

            return JWT.Encode(json, key, JwsAlgorithm.ES512, headers, options: new JwtOptions { DetachPayload = true });
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
