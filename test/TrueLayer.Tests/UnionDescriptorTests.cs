using System;
using Shouldly;
using Xunit;

namespace TrueLayer.Tests
{
    public class UnionDescriptorTests
    {
        [Fact]
        public void Returns_false_for_non_union_type()
        {
            UnionDescriptor.TryCreate(typeof(string), out var descriptor).ShouldBeFalse();
        }
        
        [Fact]
        public void Creates_factory_for_valid_union_type()
        {
            var unionType = typeof(Union<string, int>);
            UnionDescriptor.TryCreate(unionType, out var factory).ShouldBeTrue();

            factory.UnionType.ShouldBe(unionType);
            
            factory.TypeFactories.TryGetValue(nameof(String), out var unionStringFactory)
                .ShouldBeTrue();
            
            var unionString = ((Func<object, Union<string, int>>)unionStringFactory.Factory).Invoke("Test");
            unionString.Value.ShouldBe("Test");

            factory.TypeFactories.TryGetValue(nameof(Int32), out var unionIntFactory)
                .ShouldBeTrue();
            
            var unionInt = ((Func<object, Union<string, int>>)unionIntFactory.Factory).Invoke(10);
            unionInt.Value.ShouldBe(10);
        }
    }
}
