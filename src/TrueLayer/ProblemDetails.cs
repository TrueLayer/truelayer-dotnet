using System.Collections.Generic;

namespace TrueLayer;

/// <summary>
/// Represents a problem that occurred during the processing of a request
/// formatted as application/problem+json as per https://datatracker.ietf.org/doc/html/rfc7807
/// </summary>
/// <param name="Type">An absolute URI that identifies the problem type</param>
/// <param name="Title">A short, summary of the problem type</param>
/// <param name="Detail">A human readable explanation specific to this occurrence of the problem</param>
/// <param name="Instance">An absolute URI that identifies the specific occurrence of the problem</param>
/// <param name="Errors">Parameter errors for requests that fail validation</param>
public record ProblemDetails(string Type, string Title, string? Detail, string? Instance, Dictionary<string, string[]>? Errors);