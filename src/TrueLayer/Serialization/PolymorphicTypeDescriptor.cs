using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace TrueLayer.Serialization
{
    internal sealed class PolymorphicTypeDescriptor
    {
        // Needs to have discriminator field name to override with [JsonDiscriminator]
        //public static bool TryCreate(Type type);

        private PolymorphicTypeDescriptor(Type baseType, Dictionary<string, Type> typeMap, string? discriminator)
        {
            BaseType = baseType;
            TypeMap = typeMap;
            Discriminator = discriminator;
        }

        public Type BaseType { get; }
        public Dictionary<string, Type> TypeMap { get; }
        public string? Discriminator { get; }

        public static bool IsValidType(Type baseType, [NotNullWhen(true)] out IEnumerable<JsonKnownTypeAttribute>? attributes)
        {
            attributes = default;

            // TODO can we relax this restriction and just check for sealed types?
            if (!baseType.IsClass || !(baseType.IsAbstract || baseType.IsInterface))
                return false;

            attributes = baseType.GetCustomAttributes<JsonKnownTypeAttribute>(false);
            return attributes.Any();
        }

        public static bool TryCreate(Type baseType, [NotNullWhen(true)] out PolymorphicTypeDescriptor? descriptor)
        {
            descriptor = default;

            if (!IsValidType(baseType, out var attributes))
                return false;

            var typeMap = new Dictionary<string, Type>();

            foreach (var attr in attributes)
            {
                if (!baseType.IsAssignableFrom(attr.SubType))
                {
                    throw new ArgumentException($"Sub-type {attr.SubType.FullName} does not derive from {baseType.FullName}");
                }

                typeMap.Add(attr.Identifier, attr.SubType);
            }

            string? discriminator = baseType.GetCustomAttribute<JsonDiscriminatorAttribute>()?.Discriminator;

            descriptor = new PolymorphicTypeDescriptor(baseType, typeMap, discriminator);

            return true;
        }
    }
}
