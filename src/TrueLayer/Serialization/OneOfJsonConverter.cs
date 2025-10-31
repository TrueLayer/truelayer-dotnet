using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace TrueLayer.Serialization;

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

        // Extract discriminators without allocating JsonDocument
        (string? discriminatorValue, string? statusValue, bool isEmpty) = ExtractDiscriminators(ref reader, _discriminatorFieldName);

        if (isEmpty)
        {
            return default;
        }

        // Try primary discriminator field (type)
        if (!string.IsNullOrWhiteSpace(discriminatorValue)
            && (_descriptor.TypeFactories.TryGetValue(discriminatorValue, out var typeFactory)))
        {
            return InvokeDiscriminatorFactory(options, readerClone, typeFactory);
        }

        // Fallback to status field
        if (!string.IsNullOrWhiteSpace(statusValue)
            && (_descriptor.TypeFactories.TryGetValue(statusValue, out typeFactory)))
        {
            return InvokeDiscriminatorFactory(options, readerClone, typeFactory);
        }

        // Try combined status_type discriminator
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

        throw new JsonException($"Unknown discriminator: type='{discriminatorValue}', status='{statusValue}'");
    }

    private static (string? discriminatorValue, string? statusValue, bool isEmpty) ExtractDiscriminators(
        ref Utf8JsonReader reader, string discriminatorFieldName)
    {
        string? discriminatorValue = null;
        string? statusValue = null;
        int propertyCount = 0;

        // Read through the object to find discriminator fields
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyCount++;

                if (reader.ValueTextEquals(discriminatorFieldName))
                {
                    reader.Read();
                    discriminatorValue = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
                }
                else if (reader.ValueTextEquals("status"))
                {
                    reader.Read();
                    statusValue = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
                }
                else
                {
                    // Skip other properties
                    reader.Read();
                    // Use TrySkip to handle both complete buffers and streaming JSON
                    if (!reader.TrySkip())
                    {
                        // If TrySkip fails, we need to manually skip the value
                        reader.Skip();
                    }
                }
            }
        }

        return (discriminatorValue, statusValue, propertyCount == 0);
    }

    private static T InvokeDiscriminatorFactory(JsonSerializerOptions options, Utf8JsonReader readerClone,
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
