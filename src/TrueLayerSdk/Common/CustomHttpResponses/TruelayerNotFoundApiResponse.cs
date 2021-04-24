using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 404 (Not Found) response.
    /// </summary>
    public class TruelayerNotFoundApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerNotFoundApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerNotFoundApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.NotFound, httpResponseHeaders) { }
    }
}
