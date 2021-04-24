using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 429 (Too Many Requests) response.
    /// </summary>
    public class TruelayerTooManyRequestsApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerTooManyRequestsApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerTooManyRequestsApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base((HttpStatusCode)429, httpResponseHeaders) { }
    }
}
