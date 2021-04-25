using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 200 (OK) response.
    /// </summary>
    public class TruelayerOkApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerOkApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerOkApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.OK, httpResponseHeaders) { }
    }
}
