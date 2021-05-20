using System;
using Xunit;

namespace TrueLayer.Sdk.Tests
{
    public class GuardTests
    {
        [Fact]
        public void GreaterThan_throws_when_less_than_target()
         => Assert.Throws<ArgumentOutOfRangeException>(() => 10.GreaterThan(20, "param"));

        [Fact]
        public void GreaterThan_throws_when_equal_to_target()
         => Assert.Throws<ArgumentOutOfRangeException>(() => 10.GreaterThan(10, "param"));

        [Fact]
        public void GreaterThan_passes_when_greater_than_target()
         => 10.GreaterThan(5, "param");
    }
}
