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
    }
}
