using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization
{
    /// <summary>
    /// Converter for discriminated properties
    /// </summary>
    public class DiscriminatedJsonConverter : JsonConverter<IDiscriminated>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IDiscriminated).IsAssignableFrom(typeToConvert);
        }

        public override IDiscriminated? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
        
        public override void Write(Utf8JsonWriter writer, IDiscriminated value, JsonSerializerOptions options)
        {
            // We need to explicitly pass the type from provided value in order for the derived type members to be serialized
            // Ref https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-polymorphism
            System.Text.Json.JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
