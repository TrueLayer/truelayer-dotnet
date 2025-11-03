using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using OneOf;
using TrueLayer.Serialization;

namespace TrueLayer;

internal sealed class OneOfTypeDescriptor
{
    private OneOfTypeDescriptor(Type oneOfType, Dictionary<string, (Type, Delegate)> typeFactories)
    {
        OneOfType = oneOfType;
        TypeFactories = typeFactories;
    }

    public Type OneOfType { get; }

    /// <summary>
    /// Gets the factories that can be used to create the OneOf for a specific type
    /// </summary>
    public Dictionary<string, (Type FieldType, Delegate Factory)> TypeFactories { get; }

    public static bool TryCreate(Type oneOfType, [NotNullWhen(true)] out OneOfTypeDescriptor? factory)
    {
        factory = default;

        if (!typeof(IOneOf).IsAssignableFrom(oneOfType))
        {
            return false;
        }

        var oneOfFields = oneOfType.GetGenericArguments();

        if (oneOfFields.Length == 0)
        {
            return false;
        }

        var factories = new Dictionary<string, (Type, Delegate)>(oneOfFields.Length);

        // JSON will deserialize to object. The factory will then convert to the appropriate type
        Type factoryType = typeof(Func<,>).MakeGenericType(typeof(object), oneOfType);

        for (int index = 0; index < oneOfFields.Length; index++)
        {
            var fieldType = oneOfFields[index];
            Delegate valueFactory = CreateFactoryForType(index, factoryType, oneOfType, fieldType);

            // The discriminator name/id can be overridden with JsonDiscriminatorAttribute otherwise fallback to type name
            var discriminatorAttribute = fieldType.GetCustomAttribute<JsonDiscriminatorAttribute>();
            factories.Add(discriminatorAttribute?.Discriminator ?? fieldType.Name, (fieldType, valueFactory));
        }

        factory = new OneOfTypeDescriptor(oneOfType, factories);
        return true;
    }

    private static Delegate CreateFactoryForType(int typeIndex, Type factoryType, Type oneOfType, Type fieldType)
    {
        // OneOf defines a static FromTx method for each generic type argument
        var factoryMethod = oneOfType.GetMethod("FromT" + typeIndex.ToString(), BindingFlags.Public | BindingFlags.Static);

        if (factoryMethod is null)
            throw new ArgumentException($"Type {oneOfType.FullName} missing static method FromT{typeIndex}", nameof(factoryMethod));

        // JSON serializer will deserialize as object
        var input = Expression.Parameter(typeof(object));

        // Convert input parameters to the target OneOf field type
        var convertedParameter = Expression.Convert(input, fieldType);

        var factoryExpression = Expression.Lambda(
            factoryType,
            Expression.Call(factoryMethod, convertedParameter), input);

        return factoryExpression.Compile();
    }
}