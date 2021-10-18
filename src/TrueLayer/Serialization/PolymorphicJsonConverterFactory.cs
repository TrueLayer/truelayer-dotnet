using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization
{
    internal class PolymorphicJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // TODO cache result to avoid reflecting on every type
            return PolymorphicTypeDescriptor.IsValidType(typeToConvert, out _);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!PolymorphicTypeDescriptor.TryCreate(typeToConvert, out var descriptor))
            {
                throw new ArgumentException(); // shouldn't happen
            }

            var converterType = typeof(PolymorphicJsonConverter<>).MakeGenericType(typeToConvert);
            return Activator.CreateInstance(converterType, descriptor) as JsonConverter;
        }
    }
}
