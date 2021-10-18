using System.Collections.Generic;

namespace TrueLayer
{
    public record ProblemDetails(string Type, string Title, string? Detail, string? Instance, Dictionary<string, string[]>? Errors);
}
