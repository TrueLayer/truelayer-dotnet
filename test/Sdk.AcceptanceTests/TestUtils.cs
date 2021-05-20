using System;
using System.Threading.Tasks;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public static class TestUtils
    {
        public static async Task<T> RepeatUntil<T>(Func<Task<T>> valueProvider, Func<T, bool> predicate, int maxAttempts, TimeSpan? delay = null)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                T value = await valueProvider.Invoke();
                
                if (predicate.Invoke(value))
                {
                    return value;
                }

                if (delay.HasValue)
                {
                    await Task.Delay(delay.Value);
                }
            }

            return default;
        }
    }
}
