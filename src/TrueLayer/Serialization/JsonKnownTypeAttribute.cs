using System;

namespace TrueLayer.Serialization
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    internal class JsonKnownTypeAttribute : Attribute
    {
        public JsonKnownTypeAttribute(Type subType, string identifier)
        {
            SubType = subType.NotNull(nameof(subType));
            Identifier = identifier.NotNullOrWhiteSpace(nameof(identifier));

            if (!SubType.IsClass || SubType.IsInterface || SubType.IsAbstract)
            {
                throw new ArgumentException("SubType must be a valid class", nameof(subType));
            }
        }

        public Type SubType { get; }
        public string Identifier { get; }
    }
}
