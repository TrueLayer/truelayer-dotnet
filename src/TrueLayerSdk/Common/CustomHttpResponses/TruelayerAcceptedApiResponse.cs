using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 202 (Accepted) response.
    /// </summary>
    public class TruelayerAcceptedApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerAcceptedApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerAcceptedApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.Accepted, httpResponseHeaders) { }
    }
}
