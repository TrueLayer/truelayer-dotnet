using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 403 (Forbidden) response.
    /// </summary>
    public class TruelayerForbiddenApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerForbiddenApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerForbiddenApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.Forbidden, httpResponseHeaders) { }
    }
}
