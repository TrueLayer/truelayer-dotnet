using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TrueLayer.Common;

namespace TrueLayer
{
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

        /// <summary>
        /// Validate that the provided URI one of the configured (from the options) URIs as base address, or one of the TrueLayer ones based on the environment used.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="options">The <see cref="TrueLayerOptions"/> that contain the custom configured URIs</param>
        /// <returns>The value of <paramref name="value"/> if it is valid</returns>
        /// <exception cref="ArgumentException">Thrown when the value is not valid</exception>
        /// <example>
        /// <code>
        /// _uri = uri.HasValidBaseUri(nameof(_uri), options);
        /// </code>
        /// </example>
        internal static Uri HasValidBaseUri(this Uri? value, string name, TrueLayerOptions options)
        {
            value.NotNull(name);
            const string errorMsg = "The URI must be a valid TrueLayer API URI one of those configured in the settings.";
            bool result = value.IsLoopback // is localhost?
                          || ((options.Payments?.Uri is not null) && options.Payments!.Uri.IsBaseOf(value))
                          || ((options.Auth?.Uri is not null) && options.Auth!.Uri.IsBaseOf(value))
                          || ((options.Payments?.HppUri is not null) && options.Payments!.HppUri.IsBaseOf(value));

            if (options.UseSandbox == true)
            {
                result = result
                         || TrueLayerBaseUris.SandboxAuthBaseUri.IsBaseOf(value)
                         || TrueLayerBaseUris.SandboxApiBaseUri.IsBaseOf(value)
                         || TrueLayerBaseUris.SandboxHppBaseUri.IsBaseOf(value);
            }
            else
            {
                result = result
                         || TrueLayerBaseUris.ProdAuthBaseUri.IsBaseOf(value)
                         || TrueLayerBaseUris.ProdApiBaseUri.IsBaseOf(value)
                         || TrueLayerBaseUris.ProdHppBaseUri.IsBaseOf(value);
            }

            result.ThrowIfFalse(name, errorMsg);
            return value;
        }

        /// <summary>
        /// Validate that the provided value is not false
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="message">The message that needs to be assigned to the exception</param>
        /// <returns>The value of <paramref name="value"/> if not false</returns>
        /// <exception cref="ArgumentException">Thrown when the value is false</exception>
        /// <example>
        /// <code>
        /// _value = value.ThrowIfFalse(nameof(_value), "The value cannot be false");
        /// </code>
        /// </example>
        private static bool ThrowIfFalse(this bool value, string name, string message)
        {
            if (!value)
            {
                throw new ArgumentException(message, name);
            }

            return value;
        }
    }
}
