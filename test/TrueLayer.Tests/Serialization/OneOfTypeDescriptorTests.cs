using System;
using FluentAssertions;
using OneOf;
using Xunit;

namespace TrueLayer.Tests.Serialization;

public class OneOfTypeTypeFactoryTests
{
    [Fact]
    public void Returns_false_for_non_valid_type()
    {
        OneOfTypeDescriptor.TryCreate(typeof(string), out var descriptor).Should().BeFalse();
    }

    [Fact]
    public void Returns_false_if_no_generic_arguments()
    {
        OneOfTypeDescriptor.TryCreate(typeof(IOneOf), out var descriptor).Should().BeFalse();
    }

    [Fact]
    public void Creates_factory_for_valid_type()
    {
        var oneOfType = typeof(OneOf<string, int>);
        OneOfTypeDescriptor.TryCreate(oneOfType, out var factory).Should().BeTrue();

        if (factory is null) throw new Exception($"{nameof(factory)} is null");

        factory.OneOfType.Should().Be(oneOfType);

        factory.TypeFactories.TryGetValue(nameof(String), out var stringOneOfFactory)
            .Should().BeTrue();

        var stringOneOf = ((Func<object, OneOf<string, int>>)stringOneOfFactory.Factory).Invoke("Test");
        stringOneOf.Value.Should().Be("Test");

        factory.TypeFactories.TryGetValue(nameof(Int32), out var intOneOfFactory)
            .Should().BeTrue();

        var intOneOf = ((Func<object, OneOf<string, int>>)intOneOfFactory.Factory).Invoke(10);
        intOneOf.Value.Should().Be(10);
    }
}