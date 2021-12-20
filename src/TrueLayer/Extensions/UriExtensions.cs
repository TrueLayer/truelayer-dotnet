using System;
using System.Linq;

namespace TrueLayer.Extensions
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, params string[] segments)
        {
            string newUri = string.Join("/", new[] {uri.AbsoluteUri.TrimEnd('/')}
                .Concat(segments.Select(s => s.Trim('/'))));
            return new Uri(newUri);
        }
    }
}
