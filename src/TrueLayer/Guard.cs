using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TrueLayer;

internal static class Guard
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
    public static string NotNullOrWhiteSpace([NotNull] this string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value must be provided", name);
        }

        return value;
    }

    /// <summary>
    /// Values that the provided <paramref name="value"/> is not empty, or whitespace but can be null
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="name">The name of the argument</param>
    /// <returns>The value of <paramref name="value"/> if it is empty or whitespace</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the value is empty or whitespace
    /// </exception>
    /// <example>
    /// <code>
    /// _name = name.NotEmptyOrWhiteSpace(nameof(name));
    /// </code>
    /// </example>
    [DebuggerStepThrough]
    public static string? NotEmptyOrWhiteSpace(this string? value, string name)
    {
        if (value is not null && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace", name);
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
    public static T GreaterThan<T>([NotNull] this T value, T greaterThan, string paramName, string? comparedParamName = null) where T : IComparable
    {
        if (value.CompareTo(greaterThan) <= 0)
        {
            if (!string.IsNullOrWhiteSpace(comparedParamName))
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value must be greater than {greaterThan} ({comparedParamName} parameter)");
            }
            throw new ArgumentOutOfRangeException(paramName);
        }

        return value;
    }

    /// <summary>
    /// Validates that the provided <paramref name="value"/> is not an URL
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="name">The name of the argument</param>
    /// <returns>The value of <paramref name="value"/> if it is not an URL</returns>
    /// <exception cref="ArgumentException">Thrown when the value is an URL</exception>
    /// <example>
    /// <code>
    /// _id = id.NotAUrl(nameof(id));
    /// </code>
    /// </example>
    [DebuggerStepThrough]
    public static string? NotAUrl(this string? value, string name)
        => value is not null
           && (value.Contains(' ')
               || Uri.IsWellFormedUriString(value, UriKind.Absolute)
               || value.StartsWith('\\')
               || value.Contains('/')
               || value.Contains('.'))
            ? throw new ArgumentException("Value is malformed", name)
            : value;
}