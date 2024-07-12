using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace TrueLayer.Serialization
{
    internal sealed class OneOfJsonConverter<T> : JsonConverter<T> where T : IOneOf
    {
        private readonly OneOfTypeDescriptor _descriptor;
        private readonly string _discriminatorFieldName;

        public OneOfJsonConverter(OneOfTypeDescriptor descriptor, string discriminatorFieldName)
        {
            _descriptor = descriptor.NotNull(nameof(descriptor));
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

            if (doc.RootElement.TryGetProperty(_discriminatorFieldName, out var discriminator)
                && (_descriptor.TypeFactories.TryGetValue(discriminator.GetString()!, out var typeFactory)))
            {
                return InvokeDiscriminatorFactory(options, readerClone, typeFactory);
            }

            // Fallback to status field
            if (doc.RootElement.TryGetProperty("status", out discriminator)
                && (_descriptor.TypeFactories.TryGetValue(discriminator.GetString()!, out typeFactory)))
            {
                return InvokeDiscriminatorFactory(options, readerClone, typeFactory);
            }

            throw new JsonException($"Unknown discriminator {discriminator}");
        }

        private static T? InvokeDiscriminatorFactory(JsonSerializerOptions options, Utf8JsonReader readerClone,
            (Type FieldType, Delegate Factory) typeFactory)
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

            throw new JsonException($"Unable to execute OneOf factory for type {typeFactory.FieldType.FullName}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, value.Value.GetType(), options);
        }
    }
}
