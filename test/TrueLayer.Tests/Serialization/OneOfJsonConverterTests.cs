using System.Text.Json;
using OneOf;
using Shouldly;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class OneOfJsonConverterTests
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new OneOfJsonConverterFactory() }
        };

        [Fact]
        public void Throws_if_not_a_valid_json_object()
        {
            string json = "[]";
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, _options));
        }

        [Fact]
        public void Throws_if_discriminator_and_status_field_missing()
        {
            string json = @"{
                    ""field1"": ""Bar"",
                    ""field2"": 10
                }
            ";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, _options));
        }

        [Fact]
        public void Can_read_from_type_discriminator()
        {
            string json = @"{
                    ""type"": ""Bar"",
                    ""BarProp"": 10
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, _options);
            oneOf.AsT1.BarProp.ShouldBe(10);
            oneOf.Value.ShouldBeOfType<Bar>();
        }

        [Fact]
        public void Can_read_from_status_discriminator()
        {
            string json = @"{
                    ""status"": ""Bar"",
                    ""BarProp"": 10
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, _options);
            oneOf.AsT1.BarProp.ShouldBe(10);
            oneOf.Value.ShouldBeOfType<Bar>();
        }

        [Fact]
        public void Can_read_from_status_discriminator_Refund()
        {
            string json = @"{
                    ""status"": ""executed"",
                    ""AmountInMinor"": 1000,
                    ""ExecutedAt"": ""2021-01-01T00:00:00Z""
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<RefundPending, RefundAuthorized, RefundExecuted>>(json, _options);
            oneOf.Value.ShouldBeOfType<RefundExecuted>();
            oneOf.AsT2.AmountInMinor.ShouldBe<uint>(1000);
            oneOf.AsT2.Status.ShouldBe("executed");
            oneOf.AsT2.ExecutedAt.ShouldBe(new System.DateTime(2021, 1, 1, 0, 0, 0, System.DateTimeKind.Utc));
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

            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _options)
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

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Other>>(json, _options);
            oneOf.Value.ShouldBeOfType<Other>();
        }

        [Fact]
        public void Can_write_one_of_type()
        {
            OneOf<Foo, Bar> obj = new Foo();
            JsonSerializer.Serialize(obj, _options).ShouldNotBeNullOrWhiteSpace();
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
