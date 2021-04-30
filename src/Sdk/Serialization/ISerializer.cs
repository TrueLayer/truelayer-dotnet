using System;

namespace TrueLayer.Serialization
{
    /// <summary>
    /// Defines an interface for classes that provide serialization to and from string formats such as JSON.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the provided <paramref name="input"/> to string.
        /// </summary>
        /// <param name="input">The input to serialize.</param>
        /// <returns>The input serialized as string.</returns>
        string Serialize(object input);
        
        /// <summary>
        /// Deserializes the provided <paramref name="input"/> to the specified <paramref name="objectType"/>.
        /// </summary>
        /// <param name="input">The input to deserialize.</param>
        /// <param name="objectType">The object type to deserialize to.</param>
        /// <returns>The deserialized object.</returns>
        object Deserialize(string input, Type objectType);
    }
}
