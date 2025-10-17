using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TrueLayer
{
    /// <summary>
    /// Represents an API response with an expected response body
    /// </summary>
    /// <typeparam name="TData">The expected type for the response</typeparam>
    public class ApiResponse<TData> : ApiResponse
    {
        public ApiResponse(HttpStatusCode statusCode, string? traceId)
            : base(statusCode, traceId)
        {
        }

        public ApiResponse(ProblemDetails problemDetails, HttpStatusCode statusCode, string? traceId)
            : base(problemDetails, statusCode, traceId)
        {
        }

        public ApiResponse(TData data, HttpStatusCode statusCode, string? traceId)
            : base(statusCode, traceId)
        {
            Data = data.NotNull(nameof(data));
        }

        /// <summary>
        /// Gets the response body data if the request was successful <see cref="IsSuccessful"/>
        /// </summary>
        public TData? Data { get; }

        /// <summary>
        /// Gets whether the API request completed successfully
        /// </summary>
        /// <returns>True if successful, otherwise False</returns>
        [MemberNotNullWhen(true, nameof(Data))]
        public override bool IsSuccessful => base.IsSuccessful;

        public static implicit operator TData?(ApiResponse<TData> response) => response.Data;
    }
}
