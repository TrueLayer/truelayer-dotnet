using System;
using FluentAssertions;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using Xunit;
using static TrueLayer.Payments.Model.UltimateCounterparty;

namespace TrueLayer.Tests.Payments.Model
{
    public class SubMerchantsTests
    {
        [Fact]
        public void SubMerchants_Can_Be_Created_With_BusinessDivision()
        {
            // Arrange
            var businessDivision = new BusinessDivision("div-123", "Marketing Division");

            // Act
            var subMerchants = new SubMerchants(businessDivision);

            // Assert
            subMerchants.UltimateCounterparty.Should().NotBeNull();
            subMerchants.UltimateCounterparty.AsT0.Should().Be(businessDivision);
        }

        [Fact]
        public void SubMerchants_Can_Be_Created_With_BusinessClient()
        {
            // Arrange
            var businessClient = new BusinessClient(
                tradingName: "Acme Corp",
                registrationNumber: "12345678");

            // Act
            var subMerchants = new SubMerchants(businessClient);

            // Assert
            subMerchants.UltimateCounterparty.Should().NotBeNull();
            subMerchants.UltimateCounterparty.AsT1.Should().Be(businessClient);
        }

        [Fact]
        public void BusinessDivision_Has_Correct_Properties()
        {
            // Arrange & Act
            var businessDivision = new BusinessDivision("div-456", "Sales Division");

            // Assert
            businessDivision.Type.Should().Be("business_division");
            businessDivision.Id.Should().Be("div-456");
            businessDivision.Name.Should().Be("Sales Division");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BusinessDivision_Throws_If_Id_Is_Null_Or_Whitespace(string? invalidId)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>("id", () => 
                new BusinessDivision(invalidId!, "Valid Name"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BusinessDivision_Throws_If_Name_Is_Null_Or_Whitespace(string? invalidName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>("name", () => 
                new BusinessDivision("valid-id", invalidName!));
        }

        [Fact]
        public void BusinessClient_Has_Correct_Properties()
        {
            // Arrange & Act
            var businessClient = new BusinessClient(
                tradingName: "Test Trading",
                commercialName: "Test Commercial Ltd",
                url: "https://test.com",
                mcc: "1234",
                registrationNumber: "REG123456");

            // Assert
            businessClient.Type.Should().Be("business_client");
            businessClient.TradingName.Should().Be("Test Trading");
            businessClient.CommercialName.Should().Be("Test Commercial Ltd");
            businessClient.Url.Should().Be("https://test.com");
            businessClient.Mcc.Should().Be("1234");
            businessClient.RegistrationNumber.Should().Be("REG123456");
            businessClient.Address.Should().BeNull();
        }

        [Fact]
        public void BusinessClient_Can_Be_Created_With_Address_Instead_Of_Registration_Number()
        {
            // Arrange
            var address = new Address("London", "England", "SW1A 1AA", "GB", "10 Downing Street");

            // Act
            var businessClient = new BusinessClient(
                tradingName: "Test Trading",
                address: address);

            // Assert
            businessClient.TradingName.Should().Be("Test Trading");
            businessClient.Address.Should().Be(address);
            businessClient.RegistrationNumber.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BusinessClient_Throws_If_TradingName_Is_Null_Or_Whitespace(string? invalidTradingName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>("tradingName", () => 
                new BusinessClient(invalidTradingName!, registrationNumber: "REG123"));
        }

        [Fact]
        public void BusinessClient_Throws_If_Neither_RegistrationNumber_Nor_Address_Provided()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new BusinessClient("Valid Trading Name"));
        }

        [Fact]
        public void BusinessClient_Throws_If_TradingName_Exceeds_70_Characters()
        {
            // Arrange
            var longTradingName = new string('A', 71);

            // Act & Assert
            Assert.Throws<ArgumentException>("tradingName", () => 
                new BusinessClient(longTradingName, registrationNumber: "REG123"));
        }

        [Fact]
        public void BusinessClient_Throws_If_CommercialName_Exceeds_100_Characters()
        {
            // Arrange
            var longCommercialName = new string('B', 101);

            // Act & Assert
            Assert.Throws<ArgumentException>("commercialName", () => 
                new BusinessClient("Valid Trading", commercialName: longCommercialName, registrationNumber: "REG123"));
        }

        [Fact]
        public void BusinessClient_Throws_If_Url_Exceeds_100_Characters()
        {
            // Arrange
            var longUrl = "https://" + new string('c', 100);

            // Act & Assert
            Assert.Throws<ArgumentException>("url", () => 
                new BusinessClient("Valid Trading", url: longUrl, registrationNumber: "REG123"));
        }

        [Theory]
        [InlineData("123")]   // Too short
        [InlineData("12345")] // Too long
        [InlineData("abcd")]  // Not digits
        [InlineData("12a4")]  // Contains letters
        public void BusinessClient_Throws_If_Mcc_Is_Invalid(string invalidMcc)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>("mcc", () => 
                new BusinessClient("Valid Trading", mcc: invalidMcc, registrationNumber: "REG123"));
        }

        [Fact]
        public void BusinessClient_Throws_If_RegistrationNumber_Exceeds_35_Characters()
        {
            // Arrange
            var longRegistrationNumber = new string('R', 36);

            // Act & Assert
            Assert.Throws<ArgumentException>("registrationNumber", () => 
                new BusinessClient("Valid Trading", registrationNumber: longRegistrationNumber));
        }

        [Fact]
        public void BusinessClient_Accepts_Valid_Mcc()
        {
            // Act
            var businessClient = new BusinessClient("Valid Trading", mcc: "5411", registrationNumber: "REG123");

            // Assert
            businessClient.Mcc.Should().Be("5411");
        }
    }
}