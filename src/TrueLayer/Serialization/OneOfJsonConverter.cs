using System;
using System.Linq;
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

            if (doc.RootElement.ValueKind == JsonValueKind.Object && !doc.RootElement.EnumerateObject().Any())
            {
                return default;
            }

            doc.RootElement.TryGetProperty(_discriminatorFieldName, out var discriminator);
            string? discriminatorValue = discriminator.GetString();
            if (!string.IsNullOrWhiteSpace(discriminatorValue)
                && (_descriptor.TypeFactories.TryGetValue(discriminatorValue, out var typeFactory)))
            {
                return InvokeDiscriminatorFactory(options, readerClone, typeFactory);
            }

            // Fallback to status field
            doc.RootElement.TryGetProperty("status", out discriminator);
            string? statusValue = discriminator.GetString();
            if (!string.IsNullOrWhiteSpace(statusValue)
                && (_descriptor.TypeFactories.TryGetValue(statusValue, out typeFactory)))
            {
                return InvokeDiscriminatorFactory(options, readerClone, typeFactory);
            }

            var statusTypeDiscriminator = string.Join("_", statusValue, discriminatorValue);
            if (!string.IsNullOrWhiteSpace(statusTypeDiscriminator)
                && (_descriptor.TypeFactories.TryGetValue(statusTypeDiscriminator, out typeFactory)))
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
