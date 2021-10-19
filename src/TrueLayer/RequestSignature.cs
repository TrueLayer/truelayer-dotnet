using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Jose;

namespace TrueLayer
{
    internal static class RequestSignature
    {
        private static JwtOptions Options = new JwtOptions { DetachPayload = true };
        
        /// <summary>
        /// Create a Json Web Signature (JWS) using the provided ES512 signing certificate
        /// </summary>
        /// <param name="signingKey">The Elliptic Curve signing key</param>
        /// <param name="httpMethod">The HTTP method of the request</param>
        /// <param name="uri">The request URI</param>
        /// <param name="jsonBody">The JSON body of the request</param>
        /// <param name="idempotencyKey">The request idempotency key</param>
        /// <returns>A JSON web signature</returns>
        public static string Create(SigningKey signingKey, HttpMethod httpMethod, Uri uri, ReadOnlySpan<char> jsonBody, string? idempotencyKey = null)
        {
            signingKey.NotNull(nameof(signingKey));

            var headers = new Dictionary<string, object>
            {
                // alg is already set when provided to JWT.Encode
                { "kid", signingKey.KeyId },
                { "tl_version", "2" }
            };

            // Signing Format
            // The HTTP VERB (capitalized), followed by a space, then the absolute path (without trailing slashes) e.g.: POST /payouts, followed by a newline character \n
            // For each header specified in tl_headers, in the same order (and with the same casing):
            // The header name, followed by a colon, a space, then the header value, e.g.: Idempotency-Key: 619410b3-b00c-406e-bb1b-2982f97edb8b, followed by a newline character \n
            // The serialized HTTP request body (if sending a body)           

            var sb = new StringBuilder()
                .AppendFormat("{0} {1}\n", httpMethod.Method, uri.AbsolutePath.TrimEnd('/'));

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                headers.Add("tl_headers", CustomHeaders.IdempotencyKey);
                sb.AppendFormat("{0}: {1}\n", CustomHeaders.IdempotencyKey, idempotencyKey);
            }

            if (!jsonBody.IsEmpty)
            {
                sb.Append(jsonBody);
            }

            return JWT.Encode(sb.ToString(), signingKey.Value, JwsAlgorithm.ES512, headers, options: Options);
        }
    }
}
