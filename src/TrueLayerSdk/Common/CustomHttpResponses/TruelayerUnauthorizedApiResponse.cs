using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 401 (Unauthorized) response.
    /// </summary>
    public class TruelayerUnauthorizedApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerUnauthorizedApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerUnauthorizedApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.Unauthorized, httpResponseHeaders) { }
    }
}
