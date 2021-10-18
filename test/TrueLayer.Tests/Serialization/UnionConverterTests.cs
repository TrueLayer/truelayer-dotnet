using System.Text.Json;
using Shouldly;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class UnionConverterTests
    {
        [Fact]
        public void Can_read()
        {
            string json = @"{
                    ""type"": ""Bar"",
                    ""BarProp"": 10
                }
            ";

            var options = new JsonSerializerOptions
            {
                Converters = { new UnionConverterFactory() }
            };

            var union = JsonSerializer.Deserialize<Union<Foo, Bar>>(json, options);
            union.AsT1.BarProp.ShouldBe(10);
            union.Value.ShouldBeOfType<Bar>();
        }

        [Fact]
        public void Can_read_nested_unions()
        {
            string json = @"{
                ""Name"": ""Nested"",
                ""Union"": {
                        ""type"": ""Foo"",
                        ""FooProp"": ""test""
                    }
                }
            ";

            var options = new JsonSerializerOptions
            {
                Converters = { new UnionConverterFactory() }
            };

            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, options)
                .ShouldNotBeNull();
            wrapper.Name.ShouldBe("Nested");
            wrapper.Union.AsT0.FooProp.ShouldBe("test");
        }

        public class Foo
        {
            public string? FooProp { get; set; }
        }

        public class Bar
        {
            public int BarProp { get; set; }
        }

        public class Wrapper
        {
            public string? Name { get; set; }
            public Union<Foo, Bar> Union { get; set; }
        }
    }
}
