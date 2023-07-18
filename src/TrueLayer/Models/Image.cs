using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrueLayer.Serialization;

namespace TrueLayer.Models
{
    public static class Image
    {
        [JsonDiscriminator("base64")]
        public record Base64(string Type, string Data, string MediaType) : IDiscriminated;

        [JsonDiscriminator("uri")]
        public record Uri(string Type, [property: JsonPropertyName("uri")] string Value) : IDiscriminated;
    }
}
