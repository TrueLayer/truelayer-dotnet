namespace TrueLayer
{
    /// <summary>
    /// Container for API responses wrapped in a `result` object
    /// </summary>
    /// <param name="Result"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    internal record ApiResult<TResult>(TResult Result)
    {
        public static implicit operator TResult(ApiResult<TResult> result) => result.Result;
    }
}
