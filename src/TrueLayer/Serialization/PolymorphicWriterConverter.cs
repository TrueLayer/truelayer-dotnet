using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization
{
    /// <summary>
    /// Converter to serialize properties of derived types
    /// </summary>
    internal class PolymorphicWriterConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // Converter should not run for the concrete type
            // as it will result in an infinite loop
            return typeToConvert.IsInterface || typeToConvert.IsAbstract;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            // We need to explicitly pass the type from provided value in order for the derived type members to be serialized
            // Ref https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-polymorphism
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
