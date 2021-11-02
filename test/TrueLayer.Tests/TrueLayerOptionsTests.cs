using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace TrueLayer.Tests
{
    public class TrueLayerOptionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Invalid_if_client_id_not_provided(string? value)
        {
            var options = new TrueLayerOptions
            {
                ClientId = value,
                ClientSecret = "secret"
            };

            Assert.Throws<ValidationException>(() => options.Validate());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Invalid_if_client_secret_not_provided(string? value)
        {
            var options = new TrueLayerOptions
            {
                ClientId = "client-id",
                ClientSecret = value
            };

            Assert.Throws<ValidationException>(() => options.Validate());
        }

        [Fact]
        public void Valid_if_required_parameters_provided()
        {
            new TrueLayerOptions
            {
                ClientId = "client-id",
                ClientSecret = "secret"
            }.Validate();
        }
    }
}
