using System.Net;

namespace TrueLayer;

/// <summary>
/// Represents an API response
/// </summary>
public class ApiResponse
{
    public ApiResponse(HttpStatusCode statusCode, string? traceId)
    {
        StatusCode = statusCode;
        TraceId = traceId;
    }

    public ApiResponse(ProblemDetails problemDetails, HttpStatusCode statusCode, string? traceId)
    {
        StatusCode = statusCode;
        TraceId = traceId;
        Problem = problemDetails.NotNull(nameof(problemDetails));
    }

    /// <summary>
    /// Gets the HTTP status code of the response
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the TrueLayer trace identifier for the request
    /// </summary>
    public string? TraceId { get; }

    /// <summary>
    /// Gets the problem details if the request was unsuccessful
    /// </summary>
    public ProblemDetails? Problem { get; }

    /// <summary>
    /// Gets whether the API request completed successfully
    /// </summary>
    /// <returns>True if successful, otherwise False</returns>
    public virtual bool IsSuccessful => (int)StatusCode is (>= 200 and <= 299);
}