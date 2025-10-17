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
            string? discriminatorValue = GetStringValue(discriminator);
            if (!string.IsNullOrWhiteSpace(discriminatorValue)
                && (_descriptor.TypeFactories.TryGetValue(discriminatorValue, out var typeFactory)))
            {
                return InvokeDiscriminatorFactory(options, readerClone, typeFactory);
            }

            // Fallback to status field
            doc.RootElement.TryGetProperty("status", out discriminator);
            string? statusValue = GetStringValue(discriminator);
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

            // Fallback to the type marked with DefaultJsonDiscriminator attribute
            // This handles cases like CreatePayoutResponse.Created which has no status field
            var defaultType = _descriptor.TypeFactories.Values
                .FirstOrDefault(tf => tf.FieldType.GetCustomAttributes(typeof(DefaultJsonDiscriminatorAttribute), false).Any());

            if (defaultType != default)
            {
                return InvokeDiscriminatorFactory(options, readerClone, defaultType);
            }

            throw new JsonException($"Unknown discriminator {discriminator}");
        }

        private static string? GetStringValue(JsonElement jsonElement)
            => jsonElement.ValueKind == JsonValueKind.String ? jsonElement.GetString() : null;

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
