using System.Net;

namespace TrueLayer
{
    public class ApiResponse
    {
        internal ApiResponse(HttpStatusCode statusCode, string? traceId)
        {
            StatusCode = statusCode;
            TraceId = traceId;
        }

        internal ApiResponse(ProblemDetails problemDetails, HttpStatusCode statusCode, string? traceId)
        {
            StatusCode = statusCode;
            TraceId = traceId;
            Problem = problemDetails.NotNull(nameof(problemDetails));
        }

        public HttpStatusCode StatusCode { get; }
        public string? TraceId { get; }

        public ProblemDetails? Problem { get; }

        public bool IsSuccessful => (int)StatusCode is (>= 200 and <= 299);
    }
}
