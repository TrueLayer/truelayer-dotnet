using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization
{
    internal class UnionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IUnion).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (UnionTypeDescriptor.TryCreate(typeToConvert, out UnionTypeDescriptor? descriptor))
            {
                var converterType = typeof(UnionConverter<>).MakeGenericType(typeToConvert);
                // TODO use expression to create the converter
                return Activator.CreateInstance(converterType, descriptor, /* discriminator field */ "type") as JsonConverter;
            }

            throw new ArgumentException($"Unable to create union converter for type {typeToConvert.FullName}");
        }
    }
}
