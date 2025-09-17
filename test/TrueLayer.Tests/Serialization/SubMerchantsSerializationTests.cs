using System.Text.Json;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class SubMerchantsSerializationTests
    {
        [Fact]
        public void BusinessDivision_Should_Serialize_And_Deserialize_Correctly()
        {
            // Arrange
            var businessDivision = new SubMerchants.BusinessDivision("test-id-123", "Test Division Name");
            var subMerchants = new SubMerchants(businessDivision);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);
            var deserialized = JsonSerializer.Deserialize<SubMerchants>(json, SerializerOptions.Default);

            // Assert
            Assert.NotNull(deserialized);
            Assert.True(deserialized!.UltimateCounterparty.IsT0);
            var deserializedBusinessDivision = deserialized.UltimateCounterparty.AsT0;
            Assert.Equal("business_division", deserializedBusinessDivision.Type);
            Assert.Equal("test-id-123", deserializedBusinessDivision.Id);
            Assert.Equal("Test Division Name", deserializedBusinessDivision.Name);
        }

        [Fact]
        public void BusinessClient_With_All_Properties_Should_Serialize_And_Deserialize_Correctly()
        {
            // Arrange
            var address = new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St");
            var businessClient = new SubMerchants.BusinessClient(
                tradingName: "Test Trading Company",
                commercialName: "Test Commercial Name",
                url: "https://example.com",
                mcc: "1234",
                registrationNumber: "REG123456",
                address: address);
            var subMerchants = new SubMerchants(businessClient);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);
            var deserialized = JsonSerializer.Deserialize<SubMerchants>(json, SerializerOptions.Default);

            // Assert
            Assert.NotNull(deserialized);
            Assert.True(deserialized!.UltimateCounterparty.IsT1);
            var deserializedBusinessClient = deserialized.UltimateCounterparty.AsT1;
            Assert.Equal("business_client", deserializedBusinessClient.Type);
            Assert.Equal("Test Trading Company", deserializedBusinessClient.TradingName);
            Assert.Equal("Test Commercial Name", deserializedBusinessClient.CommercialName);
            Assert.Equal("https://example.com", deserializedBusinessClient.Url);
            Assert.Equal("1234", deserializedBusinessClient.Mcc);
            Assert.Equal("REG123456", deserializedBusinessClient.RegistrationNumber);
            Assert.NotNull(deserializedBusinessClient.Address);
            Assert.Equal("1 Hardwick St", deserializedBusinessClient.Address!.AddressLine1);
            Assert.Equal("London", deserializedBusinessClient.Address.City);
            Assert.Equal("England", deserializedBusinessClient.Address.State);
            Assert.Equal("EC1R 4RB", deserializedBusinessClient.Address.Zip);
            Assert.Equal("GB", deserializedBusinessClient.Address.CountryCode);
        }

        [Fact]
        public void BusinessClient_With_Required_Properties_Only_Should_Serialize_And_Deserialize_Correctly()
        {
            // Arrange
            var businessClient = new SubMerchants.BusinessClient("Test Trading Company");
            var subMerchants = new SubMerchants(businessClient);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);
            var deserialized = JsonSerializer.Deserialize<SubMerchants>(json, SerializerOptions.Default);

            // Assert
            Assert.NotNull(deserialized);
            Assert.True(deserialized!.UltimateCounterparty.IsT1);
            var deserializedBusinessClient = deserialized.UltimateCounterparty.AsT1;
            Assert.Equal("business_client", deserializedBusinessClient.Type);
            Assert.Equal("Test Trading Company", deserializedBusinessClient.TradingName);
            Assert.Null(deserializedBusinessClient.CommercialName);
            Assert.Null(deserializedBusinessClient.Url);
            Assert.Null(deserializedBusinessClient.Mcc);
            Assert.Null(deserializedBusinessClient.RegistrationNumber);
            Assert.Null(deserializedBusinessClient.Address);
        }

        [Fact]
        public void BusinessDivision_Should_Serialize_With_Snake_Case_Property_Names()
        {
            // Arrange
            var businessDivision = new SubMerchants.BusinessDivision("test-id-123", "Test Division Name");
            var subMerchants = new SubMerchants(businessDivision);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);

            // Assert
            Assert.Contains("\"ultimate_counterparty\"", json);
            Assert.Contains("\"type\":\"business_division\"", json);
            Assert.Contains("\"id\":\"test-id-123\"", json);
            Assert.Contains("\"name\":\"Test Division Name\"", json);
        }

        [Fact]
        public void BusinessClient_Should_Serialize_With_Snake_Case_Property_Names()
        {
            // Arrange
            var businessClient = new SubMerchants.BusinessClient(
                tradingName: "Test Trading Company",
                commercialName: "Test Commercial Name",
                url: "https://example.com",
                mcc: "1234",
                registrationNumber: "REG123456");
            var subMerchants = new SubMerchants(businessClient);

            // Act
            string json = JsonSerializer.Serialize(subMerchants, SerializerOptions.Default);

            // Assert
            Assert.Contains("\"ultimate_counterparty\"", json);
            Assert.Contains("\"type\":\"business_client\"", json);
            Assert.Contains("\"trading_name\":\"Test Trading Company\"", json);
            Assert.Contains("\"commercial_name\":\"Test Commercial Name\"", json);
            Assert.Contains("\"registration_number\":\"REG123456\"", json);
        }
    }
}