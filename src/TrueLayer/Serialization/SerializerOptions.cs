using System.Text.Json;

namespace TrueLayer.Serialization
{
    internal class SerializerOptions
    {
        public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonSnakeCaseNamingPolicy.Instance,
            Converters = { new UnionConverterFactory() }
        };
    }
}
