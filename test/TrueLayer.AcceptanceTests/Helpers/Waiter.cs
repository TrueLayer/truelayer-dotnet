using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrueLayer.AcceptanceTests.Helpers;

public static class Waiter
{
    public static Task<T> WaitAsync<T>(
        Func<Task<T>> action,
        Predicate<T> predicate,
        TimeSpan? pause = null,
        TimeSpan? timeout = null)
        => WaitAsync(action, x => Task.FromResult(predicate(x)), pause, timeout);

    public static async Task<T> WaitAsync<T>(
        Func<Task<T>> action,
        Func<T, Task<bool>> predicate,
        TimeSpan? pause = null,
        TimeSpan? timeout = null)
    {
        var stopwatch = Stopwatch.StartNew();

        T result;

        do
        {
            result = await action();
            await Task.Delay(pause.GetValueOrDefault(TimeSpan.FromSeconds(1)));
        } while (!await predicate(result) && stopwatch.Elapsed < timeout.GetValueOrDefault(TimeSpan.FromSeconds(20)));

        return result;
    }
}
