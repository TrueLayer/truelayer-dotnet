using System.Net;

namespace TrueLayer
{
    public class ApiResponse<TData>
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

        internal ApiResponse(TData data, HttpStatusCode statusCode, string? traceId)
        {
            Data = data.NotNull(nameof(data));
            StatusCode = statusCode;
            TraceId = traceId;
        }

        public HttpStatusCode StatusCode { get; }
        public string? TraceId { get; }

        // TODO NotNullWhen 
        public TData? Data { get; }

        public ProblemDetails? Problem { get; }

        public static implicit operator TData?(ApiResponse<TData> response) => response.Data;

        public bool IsSuccessful => (int)StatusCode is (>= 200 and <= 299);
    }
}
