using System;
using System.Collections.Generic;
using FluentAssertions;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using Xunit;
using static TrueLayer.Payments.Model.Beneficiary;
using static TrueLayer.Payments.Model.PaymentMethod;
using static TrueLayer.Payments.Model.Provider;
using static TrueLayer.Payments.Model.AccountIdentifier;

namespace TrueLayer.Tests.Payments
{
    public class CreatePaymentRequestTests
    {
        private readonly BankTransfer _validPaymentMethod;
        private const long ValidAmount = 10000;
        private const string ValidCurrency = "GBP";

        public CreatePaymentRequestTests()
        {
            var beneficiary = new MerchantAccount("merchant-account-id");
            var provider = new UserSelected();
            _validPaymentMethod = new BankTransfer(provider, beneficiary);
        }

        [Fact]
        public void CreatePaymentRequest_Can_Be_Created_With_Minimum_Required_Fields()
        {
            // Act
            var request = new CreatePaymentRequest(ValidAmount, ValidCurrency, _validPaymentMethod);

            // Assert
            request.AmountInMinor.Should().Be(ValidAmount);
            request.Currency.Should().Be(ValidCurrency);
            request.PaymentMethod.Should().NotBeNull();
            request.PaymentMethod.AsT0.Should().BeOfType<BankTransfer>();
            request.User.Should().BeNull();
            request.RelatedProducts.Should().BeNull();
            request.AuthorizationFlow.Should().BeNull();
            request.Metadata.Should().BeNull();
            request.RiskAssessment.Should().BeNull();
            request.SubMerchants.Should().BeNull();
        }

        [Fact]
        public void CreatePaymentRequest_Can_Be_Created_With_SubMerchants()
        {
            // Arrange
            var ultimateCounterparty = new UltimateCounterparty();
            var subMerchants = new SubMerchants(ultimateCounterparty);

            // Act
            var request = new CreatePaymentRequest(
                ValidAmount, 
                ValidCurrency, 
                _validPaymentMethod,
                subMerchants: subMerchants);

            // Assert
            request.SubMerchants.Should().Be(subMerchants);
            request.SubMerchants!.UltimateCounterparty.Should().Be(ultimateCounterparty);
        }

        [Fact]
        public void CreatePaymentRequest_Can_Be_Created_With_All_Optional_Fields()
        {
            // Arrange
            var user = new PaymentUserRequest(
                "user-id", 
                "John Doe", 
                "john@example.com", 
                "+44123456789",
                new DateTime(1990, 1, 1),
                new Address("London", "London", "EC1A 1BB", "GB", "123 Test St"));
            
            var ultimateCounterparty = new UltimateCounterparty();
            var subMerchants = new SubMerchants(ultimateCounterparty);
            var metadata = new Dictionary<string, string> { ["key"] = "value" };

            // Act
            var request = new CreatePaymentRequest(
                ValidAmount,
                ValidCurrency,
                _validPaymentMethod,
                user,
                relatedProducts: null,
                authorizationFlow: null,
                metadata,
                riskAssessment: null,
                subMerchants);

            // Assert
            request.AmountInMinor.Should().Be(ValidAmount);
            request.Currency.Should().Be(ValidCurrency);
            request.PaymentMethod.Should().NotBeNull();
            request.PaymentMethod.AsT0.Should().BeOfType<BankTransfer>();
            request.User.Should().Be(user);
            request.Metadata.Should().BeEquivalentTo(metadata);
            request.SubMerchants.Should().Be(subMerchants);
            request.SubMerchants!.UltimateCounterparty.Should().Be(ultimateCounterparty);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void CreatePaymentRequest_Throws_If_Amount_Is_Zero_Or_Negative(long invalidAmount)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>("amountInMinor", () =>
                new CreatePaymentRequest(invalidAmount, ValidCurrency, _validPaymentMethod));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void CreatePaymentRequest_Throws_If_Currency_Is_Null_Or_Whitespace(string? invalidCurrency)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>("currency", () =>
                new CreatePaymentRequest(ValidAmount, invalidCurrency!, _validPaymentMethod));
        }

        // Note: PaymentMethod cannot be null since it's a OneOf union type
        // This test would not compile if we tried to pass null
    }
}