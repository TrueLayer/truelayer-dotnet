using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TrueLayer;

/// <summary>
/// Represents an API response with an expected response body
/// </summary>
/// <typeparam name="TData">The expected type for the response</typeparam>
public class ApiResponse<TData> : ApiResponse
{
    /// <summary>
    /// Creates a new API response
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="traceId">The TrueLayer trace identifier</param>
    public ApiResponse(HttpStatusCode statusCode, string? traceId)
        : base(statusCode, traceId)
    {
    }

    /// <summary>
    /// Creates a new API response with problem details
    /// </summary>
    /// <param name="problemDetails">The problem details describing the error</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="traceId">The TrueLayer trace identifier</param>
    public ApiResponse(ProblemDetails problemDetails, HttpStatusCode statusCode, string? traceId)
        : base(problemDetails, statusCode, traceId)
    {
    }

    /// <summary>
    /// Creates a new successful API response with data
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="traceId">The TrueLayer trace identifier</param>
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

    /// <summary>
    /// Implicitly converts the API response to the response data
    /// </summary>
    /// <param name="response">The API response</param>
    public static implicit operator TData?(ApiResponse<TData> response) => response.Data;
}