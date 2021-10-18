using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization
{
    internal class SerializerOptions
    {
        public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonSnakeCaseNamingPolicy.Instance,
            Converters = { new DiscriminatedUnionConverterFactory() }
        };
    }
}
