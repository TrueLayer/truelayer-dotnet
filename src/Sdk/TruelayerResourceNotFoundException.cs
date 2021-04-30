using System.Net;

namespace TrueLayer
{
    /// <summary>
    /// Exception thrown following a HTTP 404 (Not Found) response.
    /// </summary>
    public class TrueLayerResourceNotFoundException : TrueLayerApiException
    {
        /// <summary>
        /// Createa a new <see cref="TrueLayerResourceNotFoundException"/> instance.
        /// </summary>
        /// <param name="requestId">The unique identifier of the API request.</param>
        public TrueLayerResourceNotFoundException(string requestId) 
            : base(HttpStatusCode.NotFound, requestId)
        {
        }
    }
}
