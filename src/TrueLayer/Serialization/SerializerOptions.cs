using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization;

internal static class SerializerOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new OneOfJsonConverterFactory(), new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
    };
}
