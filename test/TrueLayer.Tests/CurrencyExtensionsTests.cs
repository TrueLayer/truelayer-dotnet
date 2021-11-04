using Shouldly;
using Xunit;

namespace TrueLayer.Tests
{
    public class CurrencyExtensionsTests
    {
        [Theory]
        [InlineData(10.99, 1099)]
        [InlineData(5.501, 550)]
        [InlineData(23.989, 2399)]
        public void Can_convert_to_minor_currency_unit(decimal majorValue, long expected)
        {
            majorValue.ToMinorCurrencyUnit(2).ShouldBe(expected);
        }

        [Theory]
        [InlineData(50000, 500.00)]
        [InlineData(1099, 10.99)]
        public void Can_convert_to_major_currency_unit(long minorValue, decimal expected)
        {
            minorValue.ToMajorCurrencyUnit(2).ShouldBe(expected);
        }
    }
}
