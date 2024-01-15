using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using TrueLayer.Serialization;

namespace TrueLayer.Extensions
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, params string[] segments)
        {
            string newUri = string.Join("/", new[] { uri.AbsoluteUri.TrimEnd('/').Replace("\\", string.Empty) }
                .Concat(segments.Select(s => s.Replace("\\", string.Empty).Trim('/'))));
            return new Uri(newUri);
        }

        public static string ToJson<T>(this ApiResponse<T> response)
            => JsonSerializer.Serialize(response, SerializerOptions.Default);
    }
}
