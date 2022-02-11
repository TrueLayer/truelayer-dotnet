using System;
using Xunit;

namespace TrueLayer.Tests
{
    public class GuardTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Not_empty_or_whitespace_throws(string? value)
        {
            Assert.Throws<ArgumentException>(() => value.NotEmptyOrWhiteSpace(nameof(value)));
        }

        [Fact]
        public void Not_empty_or_whitespace_allows_null()
        {
            _ = default(string?).NotEmptyOrWhiteSpace("value");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(default(string?))]
        public void Not_null_or_whitespace_throws(string? value)
            => Assert.Throws<ArgumentException>(() => value.NotNullOrWhiteSpace(nameof(value)));

        [Fact]
        public void Not_null_throws_if_null()
            => Assert.Throws<ArgumentNullException>(() => default(string?).NotNull("value"));

        [Theory]
        [InlineData(10)]
        [InlineData(5)]
        public void Greater_than_throws_if_less_or_equal_to_value(int value)
            => Assert.Throws<ArgumentOutOfRangeException>(() => value.GreaterThan(10, nameof(value)));

        [Fact]
        public void Greater_than_does_not_throw_if_greater_than_value()
            => _ = 10.GreaterThan(5, "value");
    }
}
