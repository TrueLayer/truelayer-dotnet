using System;
using FluentAssertions;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Tests.Payments;

public class SubMerchantsTests
{
    [Fact]
    public void BusinessDivision_Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var name = "Test Division";

        // Act
        var businessDivision = new SubMerchants.BusinessDivision(id, name);

        // Assert
        businessDivision.Type.Should().Be("business_division");
        businessDivision.Id.Should().Be(id);
        businessDivision.Name.Should().Be(name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void BusinessDivision_Constructor_Should_Throw_When_Id_Is_Invalid(string? id)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessDivision(id!, "Test Division"));
        exception.ParamName.Should().Be("id");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void BusinessDivision_Constructor_Should_Throw_When_Name_Is_Invalid(string? name)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessDivision("test-id", name!));
        exception.ParamName.Should().Be("name");
    }

    [Fact]
    public void BusinessClient_Constructor_Should_Set_Required_Properties_Correctly()
    {
        // Arrange
        var tradingName = "Test Trading Name";

        // Act
        var businessClient = new SubMerchants.BusinessClient(tradingName);

        // Assert
        businessClient.Type.Should().Be("business_client");
        businessClient.TradingName.Should().Be(tradingName);
        businessClient.CommercialName.Should().BeNull();
        businessClient.Url.Should().BeNull();
        businessClient.Mcc.Should().BeNull();
        businessClient.RegistrationNumber.Should().BeNull();
        businessClient.Address.Should().BeNull();
    }

    [Fact]
    public void BusinessClient_Constructor_Should_Set_All_Properties_Correctly()
    {
        // Arrange
        var tradingName = "Test Trading Name";
        var commercialName = "Test Commercial Name";
        var url = "https://example.com";
        var mcc = "1234";
        var registrationNumber = "REG123456";
        var address = new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St");

        // Act
        var businessClient = new SubMerchants.BusinessClient(
            tradingName, commercialName, url, mcc, registrationNumber, address);

        // Assert
        businessClient.Type.Should().Be("business_client");
        businessClient.TradingName.Should().Be(tradingName);
        businessClient.CommercialName.Should().Be(commercialName);
        businessClient.Url.Should().Be(url);
        businessClient.Mcc.Should().Be(mcc);
        businessClient.RegistrationNumber.Should().Be(registrationNumber);
        businessClient.Address.Should().Be(address);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void BusinessClient_Constructor_Should_Throw_When_TradingName_Is_Invalid(string? tradingName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessClient(tradingName!));
        exception.ParamName.Should().Be("tradingName");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void BusinessClient_Constructor_Should_Throw_When_Optional_Strings_Are_Empty_Or_Whitespace(string emptyValue)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessClient(
            "Trading Name", emptyValue, emptyValue, emptyValue, emptyValue));
        exception.ParamName.Should().Be("commercialName");
    }

    [Fact]
    public void SubMerchants_Constructor_Should_Set_UltimateCounterparty_With_BusinessDivision()
    {
        // Arrange
        var businessDivision = new SubMerchants.BusinessDivision("test-id", "Test Division");

        // Act
        var subMerchants = new SubMerchants(businessDivision);

        // Assert
        subMerchants.UltimateCounterparty.IsT0.Should().BeTrue(); // BusinessDivision
        var result = subMerchants.UltimateCounterparty.AsT0;
        result.Type.Should().Be("business_division");
        result.Id.Should().Be("test-id");
        result.Name.Should().Be("Test Division");
    }

    [Fact]
    public void SubMerchants_Constructor_Should_Set_UltimateCounterparty_With_BusinessClient()
    {
        // Arrange
        var businessClient = new SubMerchants.BusinessClient("Test Trading Name");

        // Act
        var subMerchants = new SubMerchants(businessClient);

        // Assert
        subMerchants.UltimateCounterparty.IsT1.Should().BeTrue(); // BusinessClient
        var result = subMerchants.UltimateCounterparty.AsT1;
        result.Type.Should().Be("business_client");
        result.TradingName.Should().Be("Test Trading Name");
        result.CommercialName.Should().BeNull();
        result.Url.Should().BeNull();
        result.Mcc.Should().BeNull();
        result.RegistrationNumber.Should().BeNull();
        result.Address.Should().BeNull();
    }
}