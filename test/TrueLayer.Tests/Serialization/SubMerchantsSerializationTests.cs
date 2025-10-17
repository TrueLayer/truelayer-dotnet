using System.Text.Json;
using FluentAssertions;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization;

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
        deserialized.Should().NotBeNull();
        deserialized!.UltimateCounterparty.IsT0.Should().BeTrue();
        var deserializedBusinessDivision = deserialized.UltimateCounterparty.AsT0;
        deserializedBusinessDivision.Type.Should().Be("business_division");
        deserializedBusinessDivision.Id.Should().Be("test-id-123");
        deserializedBusinessDivision.Name.Should().Be("Test Division Name");
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
        deserialized.Should().NotBeNull();
        deserialized!.UltimateCounterparty.IsT1.Should().BeTrue();
        var deserializedBusinessClient = deserialized.UltimateCounterparty.AsT1;
        deserializedBusinessClient.Type.Should().Be("business_client");
        deserializedBusinessClient.TradingName.Should().Be("Test Trading Company");
        deserializedBusinessClient.CommercialName.Should().Be("Test Commercial Name");
        deserializedBusinessClient.Url.Should().Be("https://example.com");
        deserializedBusinessClient.Mcc.Should().Be("1234");
        deserializedBusinessClient.RegistrationNumber.Should().Be("REG123456");
        deserializedBusinessClient.Address.Should().NotBeNull();
        deserializedBusinessClient.Address!.AddressLine1.Should().Be("1 Hardwick St");
        deserializedBusinessClient.Address.City.Should().Be("London");
        deserializedBusinessClient.Address.State.Should().Be("England");
        deserializedBusinessClient.Address.Zip.Should().Be("EC1R 4RB");
        deserializedBusinessClient.Address.CountryCode.Should().Be("GB");
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
        deserialized.Should().NotBeNull();
        deserialized!.UltimateCounterparty.IsT1.Should().BeTrue();
        var deserializedBusinessClient = deserialized.UltimateCounterparty.AsT1;
        deserializedBusinessClient.Type.Should().Be("business_client");
        deserializedBusinessClient.TradingName.Should().Be("Test Trading Company");
        deserializedBusinessClient.CommercialName.Should().BeNull();
        deserializedBusinessClient.Url.Should().BeNull();
        deserializedBusinessClient.Mcc.Should().BeNull();
        deserializedBusinessClient.RegistrationNumber.Should().BeNull();
        deserializedBusinessClient.Address.Should().BeNull();
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
        json.Should().Contain("\"ultimate_counterparty\"");
        json.Should().Contain("\"type\":\"business_division\"");
        json.Should().Contain("\"id\":\"test-id-123\"");
        json.Should().Contain("\"name\":\"Test Division Name\"");
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
        json.Should().Contain("\"ultimate_counterparty\"");
        json.Should().Contain("\"type\":\"business_client\"");
        json.Should().Contain("\"trading_name\":\"Test Trading Company\"");
        json.Should().Contain("\"commercial_name\":\"Test Commercial Name\"");
        json.Should().Contain("\"registration_number\":\"REG123456\"");
    }
}