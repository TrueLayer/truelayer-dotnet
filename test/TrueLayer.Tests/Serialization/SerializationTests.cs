using System.Text.Json;
using FluentAssertions;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using Xunit;
using static TrueLayer.Payments.Model.UltimateCounterparty;

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
        public void SubMerchants_BusinessDivision_Serializes_To_Correct_JSON()
        {
            // Arrange
            var businessDivision = new BusinessDivision("div-123", "Marketing Division");
            var subMerchants = new SubMerchants(businessDivision);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);

            // Assert
            json.Should().Contain("\"ultimate_counterparty\"");
            json.Should().Contain("\"type\":\"business_division\"");
            json.Should().Contain("\"id\":\"div-123\"");
            json.Should().Contain("\"name\":\"Marketing Division\"");
        }

        [Fact]
        public void SubMerchants_BusinessClient_Serializes_To_Correct_JSON()
        {
            // Arrange
            var businessClient = new BusinessClient(
                tradingName: "Acme Corp",
                commercialName: "Acme Commercial Ltd",
                url: "https://acme.com",
                mcc: "5411",
                registrationNumber: "REG12345");
            var subMerchants = new SubMerchants(businessClient);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);

            // Assert
            json.Should().Contain("\"ultimate_counterparty\"");
            json.Should().Contain("\"type\":\"business_client\"");
            json.Should().Contain("\"trading_name\":\"Acme Corp\"");
            json.Should().Contain("\"commercial_name\":\"Acme Commercial Ltd\"");
            json.Should().Contain("\"url\":\"https://acme.com\"");
            json.Should().Contain("\"mcc\":\"5411\"");
            json.Should().Contain("\"registration_number\":\"REG12345\"");
        }

        [Fact]
        public void SubMerchants_BusinessDivision_Deserializes_From_JSON()
        {
            // Arrange
            string json = @"{
                ""ultimate_counterparty"": {
                    ""type"": ""business_division"",
                    ""id"": ""div-456"",
                    ""name"": ""Sales Division""
                }
            }";

            // Act
            var subMerchants = JsonSerializer.Deserialize<SubMerchants>(json, SerializerOptions.Default);

            // Assert
            subMerchants.Should().NotBeNull();
            subMerchants!.UltimateCounterparty.Should().NotBeNull();
            subMerchants.UltimateCounterparty.IsT0.Should().BeTrue();
            
            var businessDivision = subMerchants.UltimateCounterparty.AsT0;
            businessDivision.Type.Should().Be("business_division");
            businessDivision.Id.Should().Be("div-456");
            businessDivision.Name.Should().Be("Sales Division");
        }

        [Fact]
        public void SubMerchants_BusinessClient_Deserializes_From_JSON()
        {
            // Arrange
            string json = @"{
                ""ultimate_counterparty"": {
                    ""type"": ""business_client"",
                    ""trading_name"": ""Test Corp"",
                    ""commercial_name"": ""Test Commercial"",
                    ""url"": ""https://test.com"",
                    ""mcc"": ""1234"",
                    ""registration_number"": ""REG789""
                }
            }";

            // Act
            var subMerchants = JsonSerializer.Deserialize<SubMerchants>(json, SerializerOptions.Default);

            // Assert
            subMerchants.Should().NotBeNull();
            subMerchants!.UltimateCounterparty.Should().NotBeNull();
            subMerchants.UltimateCounterparty.IsT1.Should().BeTrue();
            
            var businessClient = subMerchants.UltimateCounterparty.AsT1;
            businessClient.Type.Should().Be("business_client");
            businessClient.TradingName.Should().Be("Test Corp");
            businessClient.CommercialName.Should().Be("Test Commercial");
            businessClient.Url.Should().Be("https://test.com");
            businessClient.Mcc.Should().Be("1234");
            businessClient.RegistrationNumber.Should().Be("REG789");
        }

        [Fact]
        public void CreatePaymentRequest_With_SubMerchants_Serializes_Correctly()
        {
            // Arrange
            var paymentMethod = new PaymentMethod.BankTransfer(
                new Provider.UserSelected(),
                new Beneficiary.MerchantAccount("merchant-123"));
            
            var businessDivision = new BusinessDivision("div-789", "IT Division");
            var subMerchants = new SubMerchants(businessDivision);
            
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
            json.Should().Contain("\"id\":\"div-789\"");
            json.Should().Contain("\"name\":\"IT Division\"");
        }


        record TestRecord(string RequiredField, string? OptionalField);
    }
}
