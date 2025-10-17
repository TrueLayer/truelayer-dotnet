using System;
using FluentAssertions;
using Xunit;

namespace TrueLayer.Tests;

public class CurrencyExtensionsTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10.99, 1099)]
    [InlineData(5.501, 550)]
    [InlineData(-23.989, -2399)]
    public void Can_convert_to_minor_currency_unit(decimal majorValue, long expected)
    {
        majorValue.ToMinorCurrencyUnit(2).Should().Be(expected);
    }

    [Fact]
    public void Throws_with_negative_exponent()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => default(long).ToMajorCurrencyUnit(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => default(decimal).ToMinorCurrencyUnit(-1));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(50000, 500.00)]
    [InlineData(-1099, -10.99)]
    public void Can_convert_to_major_currency_unit(long minorValue, decimal expected)
    {
        minorValue.ToMajorCurrencyUnit(2).Should().Be(expected);
    }
}