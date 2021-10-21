using System.Text.Json;

namespace TrueLayer.Serialization
{
    internal sealed class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public static JsonSnakeCaseNamingPolicy Instance { get; } = new();

        public override string ConvertName(string name) => name.ToSnakeCase();
    }
}
