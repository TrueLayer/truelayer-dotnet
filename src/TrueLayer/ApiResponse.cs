using System.Net;

namespace TrueLayer
{
    public class ApiResponse<TData>
    {
        public ApiResponse(HttpStatusCode statusCode, string? traceId)
        {
            StatusCode = statusCode;
            TraceId = traceId;
        }

        public HttpStatusCode StatusCode { get; }
        public string? TraceId { get; }

        // TODO NotNullWhen 
        public TData? Data { get; }

        public static implicit operator TData?(ApiResponse<TData> response) => response.Data;

        public bool Success => (int)StatusCode is (>= 200 and <= 299);
    }
}
