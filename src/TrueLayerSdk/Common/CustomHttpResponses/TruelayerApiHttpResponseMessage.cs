using System.Net;
using System.Net.Http.Headers;

namespace TrueLayerSdk.Common.CustomHttpResponses
{
    public class TruelayerApiHttpResponseMessage : TruelayerHttpResponseMessage
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerApiHttpResponseMessage"/> instance.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code of the API response.</param>
        /// <param name="httpResponseHeaders">The headers of the API response.</param>
        public TruelayerApiHttpResponseMessage(HttpStatusCode httpStatusCode, HttpResponseHeaders httpResponseHeaders = null) 
            : base(httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
            if(httpResponseHeaders != null)
            {
                foreach (var (key, value) in httpResponseHeaders)
                {
                    Headers.Add(key, value);
                };
            }
        }

        /// <summary>
        /// Gets the HTTP status code of the API response.
        /// </summary>
        /// <value></value>
        public HttpStatusCode HttpStatusCode { get; }
        
        /// <summary>
        /// Gets the unique identifier of the API request.
        /// </summary>
        public string RequestId { get; }
    }
}
