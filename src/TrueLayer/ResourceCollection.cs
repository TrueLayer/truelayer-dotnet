using System.Collections.Generic;

namespace TrueLayer
{
    /// <summary>
    /// Represents a collection resource
    /// </summary>
    /// <param name="Items">The collection items</param>
    /// <typeparam name="T">The type of resource</typeparam>
    public record ResourceCollection<T>(IEnumerable<T> Items);
}
