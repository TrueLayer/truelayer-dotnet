using System.Net;

namespace TrueLayerSdk.Common.Exceptions
{
    /// <summary>
    /// Exception thrown following a HTTP 404 (Not Found) response.
    /// </summary>
    public class TruelayerResourceNotFoundException : TruelayerApiException
    {
        /// <summary>
        /// Createa a new <see cref="TruelayerResourceNotFoundException"/> instance.
        /// </summary>
        /// <param name="requestId">The unique identifier of the API request.</param>
        public TruelayerResourceNotFoundException(string requestId) 
            : base(HttpStatusCode.NotFound, requestId)
        {
        }
    }
}
