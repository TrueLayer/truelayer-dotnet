using System;
using OneOf;
using Shouldly;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class OneOfTypeTypeFactoryTests
    {
        [Fact]
        public void Returns_false_for_non_valid_type()
        {
            OneOfTypeDescriptor.TryCreate(typeof(string), out var descriptor).ShouldBeFalse();
        }
        
        [Fact]
        public void Creates_factory_for_valid_type()
        {
            var oneOfType = typeof(OneOf<string, int>);
            OneOfTypeDescriptor.TryCreate(oneOfType, out var factory).ShouldBeTrue();

            factory.OneOfType.ShouldBe(oneOfType);
            
            factory.TypeFactories.TryGetValue(nameof(String), out var stringOneOfFactory)
                .ShouldBeTrue();
            
            var stringOneOf = ((Func<object, OneOf<string, int>>)stringOneOfFactory.Factory).Invoke("Test");
            stringOneOf.Value.ShouldBe("Test");

            factory.TypeFactories.TryGetValue(nameof(Int32), out var intOneOfFactory)
                .ShouldBeTrue();
            
            var intOneOf = ((Func<object, OneOf<string, int>>)intOneOfFactory.Factory).Invoke(10);
            intOneOf.Value.ShouldBe(10);
        }
    }
}
