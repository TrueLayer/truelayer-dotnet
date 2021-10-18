using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TrueLayer
{
    public class UnionDescriptor
    {
        private UnionDescriptor(Type unionType, Dictionary<string, (Type, Delegate)> typeFactories)
        {
            UnionType = unionType.NotNull(nameof(unionType));
            TypeFactories = typeFactories.NotNull(nameof(typeFactories));
        }

        public Type UnionType { get; }
        
        /// <summary>
        /// Gets the factories that can be used to create the union from a specific type
        /// </summary>
        public Dictionary<string, (Type FieldType, Delegate Factory)> TypeFactories { get; }

        public static bool TryCreate(Type unionType, [NotNullWhen(true)]out UnionDescriptor? factory)
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
                factories.Add(fieldType.Name, (fieldType, valueFactory)); // TODO support overriding the type using attribute
            }

            factory = new UnionDescriptor(unionType, factories);
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
