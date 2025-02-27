using System.Text.Json.Serialization;
using TrueLayer.Serialization;

namespace TrueLayer.Models
{
    public static class Image
    {
        /// <summary>
        /// Base64 image.
        /// </summary>
        /// <param name="Type">base64</param>
        /// <param name="Data"></param>
        /// <param name="MediaType"></param>
        [JsonDiscriminator("base64")]
        public record Base64(string Type, string Data, string MediaType) : IDiscriminated;

        /// <summary>
        /// Uri image.
        /// </summary>
        /// <param name="Type">uri</param>
        /// <param name="Value">The image Uri.</param>
        [JsonDiscriminator("uri")]
        public record Uri(string Type, [property: JsonPropertyName("uri")] string Value) : IDiscriminated;
    }
}
