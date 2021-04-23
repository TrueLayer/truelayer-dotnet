using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 400 (Bad Request) response.
    /// </summary>
    public class TruelayerBadRequestApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerBadRequestApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerBadRequestApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.BadRequest, httpResponseHeaders) { }
    }
}
