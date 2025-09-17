using System.Text.Json;
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
            Assert.Equal(10, oneOf.AsT1.BarProp);
            Assert.IsType<Bar>(oneOf.Value);
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
            Assert.Equal(10, oneOf.AsT1.BarProp);
            Assert.IsType<Bar>(oneOf.Value);
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
            Assert.Equal(10, oneOf.AsT1.BarProp);
            Assert.IsType<Bar>(oneOf.Value);
        }

        [Fact]
        public void Can_read_from_status_discriminator_Refund_Failed()
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
            Assert.IsType<RefundFailed>(oneOf.Value);
            Assert.Equal(1000u, oneOf.AsT3.AmountInMinor);
            Assert.Equal("failed", oneOf.AsT3.Status);
            Assert.Equal(new System.DateTime(2021, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), oneOf.AsT3.CreatedAt);
            Assert.Equal(new System.DateTime(2021, 1, 1, 0, 2, 0, System.DateTimeKind.Utc), oneOf.AsT3.FailedAt);
            Assert.Equal("Something bad happened", oneOf.AsT3.FailureReason);
        }

        [Fact]
        public void Can_read_from_status_discriminator_Refund_Executed()
        {
            string json = @"{
                    ""status"": ""executed"",
                    ""AmountInMinor"": 2000,
                    ""CreatedAt"": ""2021-01-01T00:00:00Z"",
                    ""ExecutedAt"": ""2021-01-01T00:05:00Z""
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<RefundPending, RefundAuthorized, RefundExecuted, RefundFailed>>(json, _options);
            Assert.IsType<RefundExecuted>(oneOf.Value);
            Assert.Equal(2000u, oneOf.AsT2.AmountInMinor);
            Assert.Equal("executed", oneOf.AsT2.Status);
            Assert.Equal(new System.DateTime(2021, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), oneOf.AsT2.CreatedAt);
            Assert.Equal(new System.DateTime(2021, 1, 1, 0, 5, 0, System.DateTimeKind.Utc), oneOf.AsT2.ExecutedAt);
        }

        [Fact]
        public void Can_deserialize_ListPaymentRefundsResponse_with_all_refund_statuses()
        {
            string json = @"{
                ""Items"": [
                    {
                        ""status"": ""pending"",
                        ""Id"": ""refund-1"",
                        ""Reference"": ""ref-1"",
                        ""AmountInMinor"": 1000,
                        ""Currency"": ""GBP"",
                        ""Metadata"": {},
                        ""CreatedAt"": ""2021-01-01T00:00:00Z""
                    },
                    {
                        ""status"": ""authorized"",
                        ""Id"": ""refund-2"",
                        ""Reference"": ""ref-2"",
                        ""AmountInMinor"": 1500,
                        ""Currency"": ""GBP"",
                        ""Metadata"": {},
                        ""CreatedAt"": ""2021-01-01T00:01:00Z""
                    },
                    {
                        ""status"": ""executed"",
                        ""Id"": ""refund-3"",
                        ""Reference"": ""ref-3"",
                        ""AmountInMinor"": 2000,
                        ""Currency"": ""GBP"",
                        ""Metadata"": {},
                        ""CreatedAt"": ""2021-01-01T00:02:00Z"",
                        ""ExecutedAt"": ""2021-01-01T00:05:00Z""
                    },
                    {
                        ""status"": ""failed"",
                        ""Id"": ""refund-4"",
                        ""Reference"": ""TUOYAP"",
                        ""AmountInMinor"": 500,
                        ""Currency"": ""GBP"",
                        ""Metadata"": {},
                        ""CreatedAt"": ""2021-01-01T00:03:00Z"",
                        ""FailedAt"": ""2021-01-01T00:04:00Z"",
                        ""FailureReason"": ""Insufficient funds""
                    }
                ]
            }";

            var response = JsonSerializer.Deserialize<ListPaymentRefundsResponse>(json, _options);
            Assert.NotNull(response);
            Assert.Equal(4, response!.Items.Count);
            
            Assert.IsType<RefundPending>(response.Items[0].Value);
            Assert.Equal("ref-1", response.Items[0].AsT0.Reference);
            
            Assert.IsType<RefundAuthorized>(response.Items[1].Value);
            Assert.Equal("ref-2", response.Items[1].AsT1.Reference);
            
            Assert.IsType<RefundExecuted>(response.Items[2].Value);
            Assert.Equal("ref-3", response.Items[2].AsT2.Reference);
            
            Assert.IsType<RefundFailed>(response.Items[3].Value);
            Assert.Equal("TUOYAP", response.Items[3].AsT3.Reference);
            Assert.Equal("Insufficient funds", response.Items[3].AsT3.FailureReason);
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
            Assert.NotNull(wrapper);
            Assert.Equal("Nested", wrapper!.Name);
            Assert.Equal("test", wrapper.OneOf.AsT0.FooProp);
        }

        [Fact]
        public void Can_override_discriminator()
        {
            string json = @"{
                    ""type"": ""_other_""
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Other>>(json, _options);
            Assert.IsType<Other>(oneOf.Value);
        }

        [Fact]
        public void Returns_default_value_for_empty_objects()
        {
            string json = @"{
                }
            ";

            var oneOf = JsonSerializer.Deserialize<OneOf<Foo, Other>>(json, _options);
            Assert.Equal(default, oneOf.Value);
        }

        [Fact]
        public void Can_write_one_of_type()
        {
            OneOf<Foo, Bar> obj = new Foo();
            Assert.False(string.IsNullOrWhiteSpace(JsonSerializer.Serialize(obj, _options)));
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
