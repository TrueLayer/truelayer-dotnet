using System.Net;
using System.Net.Http;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    /// <summary>
    /// Base class for HTTP response messages received by the Truelayer.com SDK for .NET.
    /// </summary>
    public class TruelayerHttpResponseMessage : HttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerHttpResponseMessage"/> instance with the provided HTTP status code.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code of the API response.</param>
        public TruelayerHttpResponseMessage(HttpStatusCode httpStatusCode)
            : base(httpStatusCode) { }
    }
}
