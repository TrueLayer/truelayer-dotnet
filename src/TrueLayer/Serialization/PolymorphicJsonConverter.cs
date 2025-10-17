using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrueLayer.Serialization;

internal sealed class PolymorphicJsonConverter<T> : JsonConverter<T> where T : class
{
    private readonly PolymorphicTypeDescriptor _descriptor;

    public PolymorphicJsonConverter(PolymorphicTypeDescriptor descriptor)
    {
        _descriptor = descriptor.NotNull(nameof(descriptor));
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        Utf8JsonReader readerClone = reader;

        var doc = JsonDocument.ParseValue(ref reader);

        if (!doc.RootElement.TryGetProperty(_descriptor.Discriminator ?? "type", out var discriminator))
        {
            throw new JsonException();
        }

        if (_descriptor.TypeMap.TryGetValue(discriminator.GetString()!, out var concreteType))
        {
            dynamic? obj = JsonSerializer.Deserialize(ref readerClone, concreteType, options);
            return (T?)obj;
        }

        throw new JsonException($"Unknown discriminator {discriminator}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // TOOD should we also support writing in the same way
        // forcing derived types to be explicitly declared?
        throw new NotImplementedException();
    }
}