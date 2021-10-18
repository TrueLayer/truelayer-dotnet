using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TrueLayer.Serialization;

namespace TrueLayer
{
    internal sealed class UnionTypeDescriptor
    {
        private UnionTypeDescriptor(Type unionType, Dictionary<string, (Type, Delegate)> typeFactories)
        {
            UnionType = unionType.NotNull(nameof(unionType));
            TypeFactories = typeFactories.NotNull(nameof(typeFactories));
        }

        public Type UnionType { get; }
        
        /// <summary>
        /// Gets the factories that can be used to create the union from a specific type
        /// </summary>
        public Dictionary<string, (Type FieldType, Delegate Factory)> TypeFactories { get; }

        public static bool TryCreate(Type unionType, [NotNullWhen(true)]out UnionTypeDescriptor? factory)
        {
            factory = default;
            
            if (!typeof(IUnion).IsAssignableFrom(unionType))
                return false;

            var unionFields = unionType.GetGenericArguments();

            if (unionFields.Length == 0)
                return false;

            var factories = new Dictionary<string, (Type, Delegate)>(unionFields.Length);
            
            // JSON will deserialize to object. The factory will then convert to the appropriate type
            Type factoryType = typeof(Func<,>).MakeGenericType(typeof(object), unionType);

            foreach (Type fieldType in unionFields)
            {
                Delegate valueFactory = CreateUnionValueFactory(factoryType, unionType, fieldType);
            
                // The discriminator name/id can be overridden with JsonDiscriminatorAttribute otherwise fallback to type name
                var discriminatorAttribute = fieldType.GetCustomAttributes<JsonDiscriminatorAttribute>().FirstOrDefault();
                factories.Add(discriminatorAttribute?.Discriminator ?? fieldType.Name, (fieldType, valueFactory));
            }

            factory = new UnionTypeDescriptor(unionType, factories);
            return true;
        }

        private static Delegate CreateUnionValueFactory(Type factoryType, Type unionType, Type fieldType)
        {
            var constructor = unionType.GetConstructor(new[] { fieldType });

            if (constructor is null)
                throw new ArgumentException($"Union Type {unionType.FullName} missing constructor with parameter of type ${fieldType}", nameof(constructor));

            // JSON serializer will deserialize as object
            var input = Expression.Parameter(typeof(object));
            
            // Convert input parameters to the target union field type
            var convertedParameter = Expression.Convert(input, fieldType);

            var factoryExpression = Expression.Lambda(
                factoryType,
                Expression.New(constructor, new[] { convertedParameter }), input);

            return factoryExpression.Compile();   
        }
    }
}
