using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 422 (Unprocessable Entity) response.
    /// </summary>
    public class TruelayerUnprocessableEntityApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerUnprocessableEntityApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerUnprocessableEntityApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base((HttpStatusCode)422, httpResponseHeaders) { }
    }
}
