using System.Net;

namespace TrueLayer;

/// <summary>
/// Exception type for errors resulting from API operations.
/// </summary>
public class TrueLayerApiException : TrueLayerException
{
    /// <summary>
    /// Creates a new <see cref="TrueLayerApiException"/> instance.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code of the API response.</param>
    /// <param name="traceId">The unique identifier of the API request.</param>
    /// <param name="additionalInformation">Additional details about the error.</param>
    public TrueLayerApiException(HttpStatusCode httpStatusCode, string? traceId, string? additionalInformation = null)
        : base(GenerateMessage(httpStatusCode, additionalInformation))
    {
        HttpStatusCode = httpStatusCode;
        TraceId = traceId;
    }

    /// <summary>
    /// Gets the HTTP status code of the API response.
    /// </summary>
    /// <value></value>
    public HttpStatusCode HttpStatusCode { get; }

    /// <summary>
    /// Gets the unique identifier of the API request.
    /// </summary>
    public string? TraceId { get; }

    private static string GenerateMessage(HttpStatusCode httpStatusCode, string? additionalInformation = null)
    {
        var message = $"The API response status code {(int)httpStatusCode} does not indicate success.";

        if (!string.IsNullOrWhiteSpace(additionalInformation))
            return message + " " + additionalInformation;

        return message;
    }
}