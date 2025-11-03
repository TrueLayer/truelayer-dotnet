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
                ProcessProperty(ref reader, discriminatorFieldName, ref discriminatorValue, ref statusValue);
            }
        }

        return (discriminatorValue, statusValue, propertyCount == 0);
    }

    private static void ProcessProperty(
        ref Utf8JsonReader reader,
        string discriminatorFieldName,
        ref string? discriminatorValue,
        ref string? statusValue)
    {
        // Once we have both discriminators, skip all remaining properties
        if (discriminatorValue != null && statusValue != null)
        {
            SkipPropertyValue(ref reader);
            return;
        }

        if (reader.ValueTextEquals(discriminatorFieldName))
        {
            discriminatorValue = ReadStringPropertyValue(ref reader);
        }
        else if (reader.ValueTextEquals("status"))
        {
            statusValue = ReadStringPropertyValue(ref reader);
        }
        else
        {
            SkipPropertyValue(ref reader);
        }
    }

    private static string? ReadStringPropertyValue(ref Utf8JsonReader reader)
    {
        reader.Read();
        return reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
    }

    private static void SkipPropertyValue(ref Utf8JsonReader reader)
    {
        reader.Read();
        reader.TrySkip();
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
