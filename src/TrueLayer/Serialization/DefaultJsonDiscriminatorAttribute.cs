using System;

namespace TrueLayer.Serialization;

/// <summary>
/// Marks a type as the default/fallback discriminator type when no discriminator is found in the JSON
/// </summary>
[AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal sealed class DefaultJsonDiscriminatorAttribute : Attribute
{
}