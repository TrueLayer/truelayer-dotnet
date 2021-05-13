using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TrueLayer
{
    internal static class GuardExtensions
    {
        /// <summary>
        /// Values that the provided <paramref name="value"/> is not null, empty, or whitespace
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <returns>The value of <paramref name="value"/> if it is not null, empty or whitespace</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the value is null, empty or whitespace
        /// </exception>
        /// <example>
        /// <code>
        /// _name = name.NotNullOrWhiteSpace(nameof(name));
        /// </code>
        /// </example>
        [DebuggerStepThrough]
        public static string NotNullOrWhiteSpace([NotNull] this string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value must be provided", name);
            }

            return value;
        }

        /// <summary>
        /// Validates that the provided <paramref name="value"/> is not null
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The value of <paramref name="value"/> if it is not null</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the value is null
        /// </exception>
        /// <example>
        /// <code>
        /// _customer = customer.NotNull(nameof(customer));
        /// </code>
        /// </example>
        [DebuggerStepThrough]
        public static T NotNull<T>([NotNull] this T value, string name)
        {
            return value ?? throw new ArgumentNullException(name);
        }

        [DebuggerStepThrough]
        public static T GreaterThan<T>([NotNull] this T value, T greaterThan, string paramName) where T : IComparable
        {
            if (value.CompareTo(greaterThan) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }

            return value;
        }
    }
}
