using System;
using OneOf;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class OneOfTypeTypeFactoryTests
    {
        [Fact]
        public void Returns_false_for_non_valid_type()
        {
            Assert.False(OneOfTypeDescriptor.TryCreate(typeof(string), out var descriptor));
        }

        [Fact]
        public void Returns_false_if_no_generic_arguments()
        {
            Assert.False(OneOfTypeDescriptor.TryCreate(typeof(IOneOf), out var descriptor));
        }

        [Fact]
        public void Creates_factory_for_valid_type()
        {
            var oneOfType = typeof(OneOf<string, int>);
            Assert.True(OneOfTypeDescriptor.TryCreate(oneOfType, out var factory));

            if (factory is null) throw new Exception($"{nameof(factory)} is null");

            Assert.Equal(oneOfType, factory.OneOfType);

            Assert.True(factory.TypeFactories.TryGetValue(nameof(String), out var stringOneOfFactory));

            var stringOneOf = ((Func<object, OneOf<string, int>>)stringOneOfFactory.Factory).Invoke("Test");
            Assert.Equal("Test", stringOneOf.Value);

            Assert.True(factory.TypeFactories.TryGetValue(nameof(Int32), out var intOneOfFactory));

            var intOneOf = ((Func<object, OneOf<string, int>>)intOneOfFactory.Factory).Invoke(10);
            Assert.Equal(10, intOneOf.Value);
        }
    }
}
