using System.Text.Json;
using FluentAssertions;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class SerializationTests
    {
        [Fact]
        public void Can_handle_nullable_fields_with_record_constructors()
        {
            TestRecord obj = new("Required", null);
            string json = JsonSerializer.Serialize(obj, SerializerOptions.Default);

            TestRecord? deserialized = JsonSerializer.Deserialize<TestRecord>(json, SerializerOptions.Default);
            deserialized.Should().NotBeNull();
            deserialized!.RequiredField.Should().Be(obj.RequiredField);
            deserialized.OptionalField.Should().Be(obj.OptionalField);
        }

        [Fact]
        public void Can_deserialize_resource_collection()
        {
            string json = @"{
               ""items"": [{
                   ""required_field"": ""foo""
               }]
            }";

            var records = JsonSerializer.Deserialize<ResourceCollection<TestRecord>>(json, SerializerOptions.Default);
            records.Should().NotBeNull();
            records!.Items.Should().NotBeEmpty();
        }

        [Fact]
        public void SubMerchants_Serializes_To_Correct_JSON()
        {
            // Arrange
            var ultimateCounterparty = new UltimateCounterparty();
            var subMerchants = new SubMerchants(ultimateCounterparty);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);

            // Assert
            json.Should().Contain("\"ultimate_counterparty\"");
            json.Should().Contain("\"type\":\"business_division\"");
        }

        [Fact]
        public void SubMerchants_Deserializes_From_JSON()
        {
            // Arrange
            string json = @"{
                ""ultimate_counterparty"": {
                    ""type"": ""business_division""
                }
            }";

            // Act
            var subMerchants = JsonSerializer.Deserialize<SubMerchants>(json, SerializerOptions.Default);

            // Assert
            subMerchants.Should().NotBeNull();
            subMerchants!.UltimateCounterparty.Should().NotBeNull();
            subMerchants.UltimateCounterparty.Should().BeOfType<UltimateCounterparty>();
            subMerchants.UltimateCounterparty!.Type.Should().Be("business_division");
        }

        [Fact]
        public void CreatePaymentRequest_With_SubMerchants_Serializes_Correctly()
        {
            // Arrange
            var paymentMethod = new PaymentMethod.BankTransfer(
                new Provider.UserSelected(),
                new Beneficiary.MerchantAccount("merchant-123"));
            
            var subMerchants = new SubMerchants(new UltimateCounterparty());
            
            var request = new CreatePaymentRequest(
                amountInMinor: 10000,
                currency: "GBP",
                paymentMethod: paymentMethod,
                subMerchants: subMerchants);

            // Act
            string json = JsonSerializer.Serialize(request, SerializerOptions.Default);

            // Assert
            json.Should().Contain("\"sub_merchants\"");
            json.Should().Contain("\"ultimate_counterparty\"");
            json.Should().Contain("\"type\":\"business_division\"");
        }


        record TestRecord(string RequiredField, string? OptionalField);
    }
}
