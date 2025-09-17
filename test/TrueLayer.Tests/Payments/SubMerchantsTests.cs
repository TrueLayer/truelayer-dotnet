using System;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Tests.Payments
{
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
            Assert.Equal("business_division", businessDivision.Type);
            Assert.Equal(id, businessDivision.Id);
            Assert.Equal(name, businessDivision.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BusinessDivision_Constructor_Should_Throw_When_Id_Is_Invalid(string? id)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessDivision(id!, "Test Division"));
            Assert.Equal("id", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BusinessDivision_Constructor_Should_Throw_When_Name_Is_Invalid(string? name)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessDivision("test-id", name!));
            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public void BusinessClient_Constructor_Should_Set_Required_Properties_Correctly()
        {
            // Arrange
            var tradingName = "Test Trading Name";

            // Act
            var businessClient = new SubMerchants.BusinessClient(tradingName);

            // Assert
            Assert.Equal("business_client", businessClient.Type);
            Assert.Equal(tradingName, businessClient.TradingName);
            Assert.Null(businessClient.CommercialName);
            Assert.Null(businessClient.Url);
            Assert.Null(businessClient.Mcc);
            Assert.Null(businessClient.RegistrationNumber);
            Assert.Null(businessClient.Address);
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
            Assert.Equal("business_client", businessClient.Type);
            Assert.Equal(tradingName, businessClient.TradingName);
            Assert.Equal(commercialName, businessClient.CommercialName);
            Assert.Equal(url, businessClient.Url);
            Assert.Equal(mcc, businessClient.Mcc);
            Assert.Equal(registrationNumber, businessClient.RegistrationNumber);
            Assert.Equal(address, businessClient.Address);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BusinessClient_Constructor_Should_Throw_When_TradingName_Is_Invalid(string? tradingName)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessClient(tradingName!));
            Assert.Equal("tradingName", exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void BusinessClient_Constructor_Should_Throw_When_Optional_Strings_Are_Empty_Or_Whitespace(string emptyValue)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new SubMerchants.BusinessClient(
                "Trading Name", emptyValue, emptyValue, emptyValue, emptyValue));
            Assert.Equal("commercialName", exception.ParamName);
        }

        [Fact]
        public void SubMerchants_Constructor_Should_Set_UltimateCounterparty_With_BusinessDivision()
        {
            // Arrange
            var businessDivision = new SubMerchants.BusinessDivision("test-id", "Test Division");

            // Act
            var subMerchants = new SubMerchants(businessDivision);

            // Assert
            Assert.True(subMerchants.UltimateCounterparty.IsT0); // BusinessDivision
            var result = subMerchants.UltimateCounterparty.AsT0;
            Assert.Equal("business_division", result.Type);
            Assert.Equal("test-id", result.Id);
            Assert.Equal("Test Division", result.Name);
        }

        [Fact]
        public void SubMerchants_Constructor_Should_Set_UltimateCounterparty_With_BusinessClient()
        {
            // Arrange
            var businessClient = new SubMerchants.BusinessClient("Test Trading Name");

            // Act
            var subMerchants = new SubMerchants(businessClient);

            // Assert
            Assert.True(subMerchants.UltimateCounterparty.IsT1); // BusinessClient
            var result = subMerchants.UltimateCounterparty.AsT1;
            Assert.Equal("business_client", result.Type);
            Assert.Equal("Test Trading Name", result.TradingName);
            Assert.Null(result.CommercialName);
            Assert.Null(result.Url);
            Assert.Null(result.Mcc);
            Assert.Null(result.RegistrationNumber);
            Assert.Null(result.Address);
        }
    }
}