using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TrueLayer.Serialization;

namespace TrueLayer.Extensions
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, params string?[] segments)
        {
            string[] notNullSegments = segments.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray()!;
            string newUri = string.Join("/", new[] { uri.AbsoluteUri.TrimEnd('/').Replace("\\", string.Empty) }
                .Concat(notNullSegments.Select(s => s.Replace("\\", string.Empty).Trim('/'))));
            return new Uri(newUri);
        }

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

        public static string ToJson<T>(this ApiResponse<T> response)
            => JsonSerializer.Serialize(response, SerializerOptions.Default);
    }
}
