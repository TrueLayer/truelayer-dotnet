using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrueLayer.Serialization;

namespace TrueLayer.Models
{
    public static class RedirectStatus
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Type { Supported = 0, NotSupported = 1 }

        [JsonDiscriminator("supported")]
        public record SupportedRedirectStatus(Type Status, Uri ReturnUri);

        [JsonDiscriminator("not_supported")]
        public record NotSupportedRedirectStatus(Type Status);
    }
}
