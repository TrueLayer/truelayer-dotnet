using System.Text.Json;
using FluentAssertions;
using OneOf;
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
            oneOf.AsT1.BarProp.Should().Be(10);
            oneOf.Value.Should().BeOfType<Bar>();
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
            oneOf.AsT1.BarProp.Should().Be(10);
            oneOf.Value.Should().BeOfType<Bar>();
        }

        [Fact]
        public void Can_fallback_to_status_discriminator_when_type_discriminator_does_not_match()
        {
            string json = @"{
                    ""type"": ""NotMatched"",
                    ""status"": ""Bar"",
                    ""BarProp"": 10
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, _options);
            oneOf.AsT1.BarProp.Should().Be(10);
            oneOf.Value.Should().BeOfType<Bar>();
        }

        [Fact]
        public void Can_read_from_status_discriminator_Refund()
        {
            string json = @"{
                    ""status"": ""failed"",
                    ""AmountInMinor"": 1000,
                    ""CreatedAt"": ""2021-01-01T00:00:00Z"",
                    ""FailedAt"": ""2021-01-01T00:02:00Z"",
                    ""FailureReason"": ""Something bad happened""
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<RefundPending, RefundAuthorized, RefundExecuted, RefundFailed>>(json, _options);
            oneOf.Value.Should().BeOfType<RefundFailed>();
            oneOf.AsT3.AmountInMinor.Should().Be(1000);
            oneOf.AsT3.Status.Should().Be("failed");
            oneOf.AsT3.CreatedAt.Should().Be(new System.DateTime(2021, 1, 1, 0, 0, 0, System.DateTimeKind.Utc));
            oneOf.AsT3.FailedAt.Should().Be(new System.DateTime(2021, 1, 1, 0, 2, 0, System.DateTimeKind.Utc));
            oneOf.AsT3.FailureReason.Should().Be("Something bad happened");
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

            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _options);
            wrapper.Should().NotBeNull();
            wrapper!.Name.Should().Be("Nested");
            wrapper.OneOf.AsT0.FooProp.Should().Be("test");
        }

        [Fact]
        public void Can_override_discriminator()
        {
            string json = @"{
                    ""type"": ""_other_""
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Other>>(json, _options);
            oneOf.Value.Should().BeOfType<Other>();
        }

        [Fact]
        public void Returns_default_value_for_empty_objects()
        {
            string json = @"{
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Other>>(json, _options);
            oneOf.Value.Should().Be(default);
        }

        [Fact]
        public void Can_write_one_of_type()
        {
            OneOf<Foo, Bar> obj = new Foo();
            JsonSerializer.Serialize(obj, _options).Should().NotBeNullOrWhiteSpace();
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
