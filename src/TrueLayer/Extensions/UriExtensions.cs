using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueLayer.Extensions;

/// <summary>
/// Extension methods for URI manipulation
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Appends path segments to the URI
    /// </summary>
    /// <param name="uri">The base URI</param>
    /// <param name="segments">The path segments to append</param>
    /// <returns>A new URI with the appended segments</returns>
    public static Uri Append(this Uri uri, params string?[] segments)
    {
        string[] notNullSegments = segments.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray()!;
        string newUri = string.Join("/", new[] { uri.AbsoluteUri.TrimEnd('/').Replace("\\", string.Empty) }
            .Concat(notNullSegments.Select(s => s.Replace("\\", string.Empty).Trim('/'))));
        return new Uri(newUri);
    }

    /// <summary>
    /// Appends query parameters to the URI
    /// </summary>
    /// <param name="uri">The base URI</param>
    /// <param name="queryParams">The query parameters to append</param>
    /// <returns>A new URI with the appended query parameters</returns>
    public static Uri AppendQueryParameters(this Uri uri, IDictionary<string, string?>? queryParams)
    {
        if (queryParams == null || !queryParams.Any())
        {
            return uri;
        }

        var query = string.Join("&", queryParams
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && kvp.Value != null)
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={kvp.Value!.Trim()}"));

        var uriBuilder = new UriBuilder(uri)
        {
            Query = string.IsNullOrWhiteSpace(uri.Query) ? query : $"{uri.Query.TrimStart('?')}&{query}"
        };

        return uriBuilder.Uri;
    }
}