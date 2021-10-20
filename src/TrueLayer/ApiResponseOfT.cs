using System.Diagnostics.CodeAnalysis;
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

        public TData? Data { get; }

#if NET5_0 || NET5_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Data))]
#endif
        public override bool IsSuccessful => base.IsSuccessful;

        public static implicit operator TData?(ApiResponse<TData> response) => response.Data;
    }
}
