using System.Net;

namespace TrueLayer
{
    public class ApiResponse<TData> : ApiResponse
    {
        internal ApiResponse(HttpStatusCode statusCode, string? traceId)
            : base(statusCode, traceId)
        {

        }

        internal ApiResponse(ProblemDetails problemDetails, HttpStatusCode statusCode, string? traceId)
            : base(problemDetails, statusCode, traceId)
        {
        }

        internal ApiResponse(TData data, HttpStatusCode statusCode, string? traceId)
            : base(statusCode, traceId)
        {
            Data = data.NotNull(nameof(data));
        }

        //[MemberNotNullWhen(true, nameof(ApiResponse.IsSuccessful))]
        public TData? Data { get; }

        public static implicit operator TData?(ApiResponse<TData> response) => response.Data;
    }
}
