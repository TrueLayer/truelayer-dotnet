using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization
{
    internal class DiscriminatedUnionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // TODO cache result to avoid reflecting on every type
            return DiscriminatedUnionDescriptor.IsValidType(typeToConvert, out _);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!DiscriminatedUnionDescriptor.TryCreate(typeToConvert, out var descriptor))
            {
                throw new ArgumentException(); // shouldn't happen
            }

            var converterType = typeof(DiscriminatedUnionConverter<>).MakeGenericType(typeToConvert);
            return Activator.CreateInstance(converterType, descriptor) as JsonConverter;
        }
    }
}
