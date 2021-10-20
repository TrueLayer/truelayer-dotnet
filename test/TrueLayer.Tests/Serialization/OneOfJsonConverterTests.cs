using System.Text.Json;
using OneOf;
using Shouldly;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class OneOfJsonConverterTests
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
                Converters = { new OneOfJsonConverterFactory() }
            };

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, options);
            oneOf.AsT1.BarProp.ShouldBe(10);
            oneOf.Value.ShouldBeOfType<Bar>();
        }

        [Fact]
        public void Can_read_nested()
        {
            string json = @"{
                ""Name"": ""Nested"",
                ""OneOf"": {
                        ""type"": ""Foo"",
                        ""FooProp"": ""test""
                    }
                }
            ";

            var options = new JsonSerializerOptions
            {
                Converters = { new OneOfJsonConverterFactory() }
            };

            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, options)
                .ShouldNotBeNull();
            wrapper.Name.ShouldBe("Nested");
            wrapper.OneOf.AsT0.FooProp.ShouldBe("test");
        }

        [Fact]
        public void Can_override_discriminator()
        {
            string json = @"{
                    ""type"": ""_other_""
                }
            ";

            var options = new JsonSerializerOptions
            {
                Converters = { new OneOfJsonConverterFactory() }
            };

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Other>>(json, options);
            oneOf.Value.ShouldBeOfType<Other>();
        }

        public class Foo
        {
            public string? FooProp { get; set; }
        }

        public class Bar
        {
            public int BarProp { get; set; }
        }

        [JsonDiscriminator("_other_")]
        public class Other
        {

        }

        public class Wrapper
        {
            public string? Name { get; set; }
            public OneOf<Foo, Bar> OneOf { get; set; }
        }
    }
}
