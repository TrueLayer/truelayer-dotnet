using System.Collections.Generic;

namespace TrueLayer
{
    /// <summary>
    /// Container for API array responses wrapped in a `results` object
    /// </summary>
    /// <param name="Results"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    internal record ApiResultCollection<TResult>(IEnumerable<TResult> Results)
    {
    }
}
