using System.Collections.Generic;

namespace TrueLayer
{
    /// <summary>
    /// Represents the results of a query endpoint
    /// </summary>
    public record QueryResponse<TResult>
    {
        /// <summary>
        /// Gets the query results
        /// /// </summary>
        public IEnumerable<TResult> Results { get; init; } = null!;
    }
}
