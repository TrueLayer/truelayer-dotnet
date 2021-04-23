using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 500 (Internal Server Error) response.
    /// </summary>
    public class TruelayerInternalServerErrorApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerInternalServerErrorApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerInternalServerErrorApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.InternalServerError, httpResponseHeaders) { }
    }
}
