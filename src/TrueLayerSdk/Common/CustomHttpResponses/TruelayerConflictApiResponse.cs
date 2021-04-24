using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Truelayer API HTTP response message following a HTTP 409 (Conflict) response.
    /// </summary>
    public class TruelayerConflictApiResponse : TruelayerApiHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerConflictApiResponse"/> instance.
        /// </summary>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerConflictApiResponse(HttpResponseHeaders httpResponseHeaders = null) 
            : base(HttpStatusCode.Conflict, httpResponseHeaders) { }
    }
}
