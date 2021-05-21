using System;
using System.Collections.Generic;
using Xunit;

namespace TrueLayer.Sdk.Tests
{
    public class GuardExtensionsTests
    {
        [Theory]
        [MemberData(nameof(ListCases))]
        public void ShouldThrow_WithNullOrEmptyList(List<string> list)
        {
            // Act
            var ex = Record.Exception(() => list.NotEmpty(nameof(list)));
            // Assert
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal($"Sequence '{nameof(list)}' contains no elements", ex.Message);
        }

        [Fact]
        public void ShouldNotThrow_WithPopulatedList()
        {
            // Arrange
            var list = new List<string> {"something"};
            // Act
            var ex = Record.Exception(() => list.NotEmpty(nameof(list)));
            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void GreaterThan_throws_when_less_than_target()
            => Assert.Throws<ArgumentOutOfRangeException>(() => 10.GreaterThan(20, "param"));

        [Fact]
        public void GreaterThan_throws_when_equal_to_target()
            => Assert.Throws<ArgumentOutOfRangeException>(() => 10.GreaterThan(10, "param"));

        [Fact]
        public void GreaterThan_passes_when_greater_than_target()
            => 10.GreaterThan(5, "param");

        public static IEnumerable<object[]> ListCases()
        {
            var listCases = new List<object[]>
            {
                new object[] { null },
                new object[] { new List<string>() },
            };

            return listCases;
        }
    }
}
