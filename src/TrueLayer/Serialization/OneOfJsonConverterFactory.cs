using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace TrueLayer.Serialization
{
    internal sealed class OneOfJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IOneOf).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (OneOfTypeDescriptor.TryCreate(typeToConvert, out OneOfTypeDescriptor? descriptor))
            {
                var converterType = typeof(OneOfJsonConverter<>).MakeGenericType(typeToConvert);
                // TODO use expression to create the converter
                return Activator.CreateInstance(converterType, descriptor, /* discriminator field */ "type") as JsonConverter;
            }

            throw new ArgumentException($"Unable to create OneOf converter for type {typeToConvert.FullName}");
        }
    }
}
