using System.Net;

namespace TrueLayer
{
    /// <summary>
    /// Exception thrown following a HTTP 422 (Unprocessable) response.
    /// </summary>
    public class TrueLayerValidationException : TrueLayerApiException
    {
        /// <summary>
        /// 
        /// /// </summary>
        /// <param name="error">The validation error details.</param>
        /// <param name="httpStatusCode">The HTTP status code of the API response.</param>
        /// <param name="requestId">The unique identifier of the API request.</param>
        public TrueLayerValidationException(ErrorResponse error, HttpStatusCode httpStatusCode, string requestId)
         : base(httpStatusCode, requestId, GenerateDetailsMessage(error))
        {
            Error = error;
        }

        /// <summary>
        /// Gets the error response.
        /// </summary>
        public ErrorResponse Error { get; }

        private static string GenerateDetailsMessage(ErrorResponse error)
            => error is null
                ? "An unspecified validation error occurred"
                : $"A validation error of type {error.Error} occurred with error codes [{string.Join(",", error.ErrorDetails.Parameters)}].";
    }
}
