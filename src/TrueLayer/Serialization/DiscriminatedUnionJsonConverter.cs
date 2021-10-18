using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization
{
    internal class DiscriminatedUnionJsonConverter<T> : JsonConverter<T> where T : IUnion
    {
        private readonly UnionTypeDescriptor _factory;
        private readonly string _discriminatorFieldName;

        public DiscriminatedUnionJsonConverter(UnionTypeDescriptor factory, string discriminatorFieldName)
        {
            _factory = factory.NotNull(nameof(factory));
            _discriminatorFieldName = discriminatorFieldName.NotNullOrWhiteSpace(nameof(discriminatorFieldName));
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            Utf8JsonReader readerClone = reader;

            var doc = JsonDocument.ParseValue(ref reader);

            if (!doc.RootElement.TryGetProperty(_discriminatorFieldName, out var discriminator) 
                && !doc.RootElement.TryGetProperty("status", out discriminator)) // Hack until payment response uses a `type` field
            {
                throw new JsonException();
            }

            if (_factory.TypeFactories.TryGetValue(discriminator.GetString()!, out var typeFactory))
            {
                object? deserializedObject = JsonSerializer.Deserialize(ref readerClone, typeFactory.FieldType, options);

                if (deserializedObject is null)
                {
                    throw new ArgumentNullException(nameof(deserializedObject));
                }

                if (typeFactory.Factory is Func<object, T> factory)
                {
                    return factory.Invoke(deserializedObject);
                }

                throw new ArgumentException($"Unable to execute union type factory for type {typeFactory.FieldType.FullName}");
            }

            throw new JsonException($"Unknown discriminator {discriminator}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, value.Value.GetType(), options);
        }
    }
}
