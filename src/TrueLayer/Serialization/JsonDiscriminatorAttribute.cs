using System;

namespace TrueLayer.Serialization
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    internal class JsonDiscriminatorAttribute : Attribute
    {
        public JsonDiscriminatorAttribute(string discriminator)
        {
            Discriminator = discriminator.NotNullOrWhiteSpace(nameof(discriminator));
        }

        public string Discriminator { get; }
    }
}
