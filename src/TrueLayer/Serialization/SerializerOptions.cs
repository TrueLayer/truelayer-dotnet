using System.Text.Json;

namespace TrueLayer.Serialization
{
    internal sealed class SerializerOptions
    {
        public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonSnakeCaseNamingPolicy.Instance,
            Converters = { new OneOfJsonConverterFactory() }
        };
    }
}
