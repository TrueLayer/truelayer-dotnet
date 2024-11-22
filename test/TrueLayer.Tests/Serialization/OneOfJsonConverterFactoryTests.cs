using System;
using FluentAssertions;
using OneOf;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class OneOfJsonConverterFactoryTests
    {
        [Fact]
        public void Can_create_one_of_converter()
            => new OneOfJsonConverterFactory()
                .CreateConverter(typeof(OneOf<string, int>), new()).Should().NotBeNull();

        [Fact]
        public void Throws_if_invalid_oneof_type()
            => Assert.Throws<ArgumentException>(() => new OneOfJsonConverterFactory().CreateConverter(typeof(string), new()));

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(IOneOf), true)]
        [InlineData(typeof(OneOf<string, int>), true)]
        public void Can_convert_only_one_of_types(Type type, bool expected)
            => new OneOfJsonConverterFactory().CanConvert(type).Should().Be(expected);
    }
}
