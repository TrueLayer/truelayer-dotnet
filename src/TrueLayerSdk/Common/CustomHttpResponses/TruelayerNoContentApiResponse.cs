using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 204 (No Content) response.
    /// </summary>
    public class TruelayerNoContentApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerNoContentApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerNoContentApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.NoContent, httpResponseHeaders) { }
    }
}
