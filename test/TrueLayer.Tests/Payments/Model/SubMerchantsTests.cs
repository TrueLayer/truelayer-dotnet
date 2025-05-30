using FluentAssertions;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Tests.Payments.Model
{
    public class SubMerchantsTests
    {
        [Fact]
        public void SubMerchants_Can_Be_Created_With_Null_UltimateCounterparty()
        {
            // Act
            var subMerchants = new SubMerchants();

            // Assert
            subMerchants.UltimateCounterparty.Should().BeNull();
        }

        [Fact]
        public void SubMerchants_Can_Be_Created_With_UltimateCounterparty()
        {
            // Arrange
            var ultimateCounterparty = new UltimateCounterparty();

            // Act
            var subMerchants = new SubMerchants(ultimateCounterparty);

            // Assert
            subMerchants.UltimateCounterparty.Should().Be(ultimateCounterparty);
        }

        [Fact]
        public void UltimateCounterparty_Has_Correct_Type()
        {
            // Act
            var ultimateCounterparty = new UltimateCounterparty();

            // Assert
            ultimateCounterparty.Type.Should().Be("business_division");
        }

        [Fact]
        public void UltimateCounterparty_Can_Be_Created_Successfully()
        {
            // Act
            var ultimateCounterparty = new UltimateCounterparty();

            // Assert
            ultimateCounterparty.Should().NotBeNull();
            ultimateCounterparty.Type.Should().Be("business_division");
        }
    }
}