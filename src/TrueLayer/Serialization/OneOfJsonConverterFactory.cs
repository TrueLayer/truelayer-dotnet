using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace TrueLayer.Serialization;

internal sealed class OneOfJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, JsonConverter> _converterCache = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IOneOf).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions _)
    {
        return _converterCache.GetOrAdd(typeToConvert, CreateConverterForType);
    }

    private static JsonConverter CreateConverterForType(Type typeToConvert)
    {
        if (!OneOfTypeDescriptor.TryCreate(typeToConvert, out OneOfTypeDescriptor? descriptor))
        {
            throw new ArgumentException($"Unable to create OneOf converter for type {typeToConvert.FullName}");
        }

        var converterType = typeof(OneOfJsonConverter<>).MakeGenericType(typeToConvert);
        var constructor = converterType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            null,
            new[] { typeof(OneOfTypeDescriptor), typeof(string) },
            null);

        if (constructor == null)
        {
            throw new InvalidOperationException($"Could not find constructor for {converterType.FullName}");
        }

        // Use compiled expression instead of Activator.CreateInstance for better performance
        var descriptorParam = Expression.Constant(descriptor, typeof(OneOfTypeDescriptor));
        var discriminatorParam = Expression.Constant("type", typeof(string));
        var newExpression = Expression.New(constructor, descriptorParam, discriminatorParam);
        var lambda = Expression.Lambda<Func<JsonConverter>>(newExpression);
        var factory = lambda.Compile();

        return factory();
    }
}