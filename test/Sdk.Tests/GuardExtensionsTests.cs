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
            var ex = Record.Exception(() => list.NotEmptyList(nameof(list)));
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
            var ex = Record.Exception(() => list.NotEmptyList(nameof(list)));
            // Assert
            Assert.Null(ex);
        }
        
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
