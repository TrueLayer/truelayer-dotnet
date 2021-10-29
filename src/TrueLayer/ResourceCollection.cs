using System.Collections.Generic;

namespace TrueLayer
{
    /// <summary>
    /// Container for resource collection wrapped in an `items` array
    /// </summary>
    /// <param name="Items"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public record ResourceCollection<T>(IEnumerable<T> Items)
    {

    }
}
