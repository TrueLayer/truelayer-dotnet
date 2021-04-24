using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 502 (Bad Gateway) response.
    /// </summary>
    public class TruelayerBadGatewayApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerBadGatewayApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerBadGatewayApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.BadGateway, httpResponseHeaders) { }
    }
}
