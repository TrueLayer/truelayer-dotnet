using System;

namespace TrueLayer;

/// <summary>
/// Extensions for converting amounts
/// </summary>
public static class CurrencyExtensions
{
    /// <summary>
    /// Converts a minor currency unit value into the major currency unit (e.g. 100 cents > 1.00 EUR)
    /// based on the <paramref name="currencyExponent" />.
    /// </summary>
    /// <param name="minorValue">The value in minor units</param>
    /// <param name="currencyExponent">The number of decimal places between the smallest defined currency unit and a whole currency unit. For most currencies this number is 2, representing the cent.</param>
    /// <returns>The converted value</returns>
    public static decimal ToMajorCurrencyUnit(this long minorValue, int currencyExponent = 2)
    {
        if (currencyExponent < 0)
            throw new ArgumentOutOfRangeException(nameof(currencyExponent), "Currency exponent value cannot be negative");

        if (minorValue == 0)
            return 0;

        return minorValue / (decimal)Math.Pow(10.0, currencyExponent);
    }

    /// <summary>
    /// Converts a major currency unit value into the minor currency unit (e.g. 1.00 EUR > 100 cents)
    /// based on the <paramref name="currencyExponent" />.
    /// </summary>
    /// <param name="majorValue">The value in major units</param>
    /// <param name="currencyExponent">The number of decimal places between the smallest defined currency unit and a whole currency unit. For most currencies this number is 2, representing the cent.</param>
    /// <returns>The converted value</returns>
    public static long ToMinorCurrencyUnit(this decimal majorValue, int currencyExponent)
    {
        if (currencyExponent < 0)
            throw new ArgumentOutOfRangeException(nameof(currencyExponent), "Currency exponent value cannot be negative");

        if (majorValue == 0)
            return 0;

        return Convert.ToInt64(majorValue * (decimal)Math.Pow(10.0, currencyExponent));
    }
}