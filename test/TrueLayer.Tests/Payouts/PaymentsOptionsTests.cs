using System;
using System.ComponentModel.DataAnnotations;
using TrueLayer.Payouts;
using Xunit;

namespace TrueLayer.Tests.Payouts
{
    public class PayoutsOptionsTests
    {
        private static Uri TestUri = new("http://api.truelayer.com");

        [Fact]
        public void Invalid_if_no_signing_key()
        {
            Assert.Throws<ValidationException>(() => new PayoutsOptions { Uri = TestUri }.Validate());
        }

        [Fact]
        public void Invalid_if_no_signing_key_id()
        {
            Assert.Throws<ValidationException>(() =>
                new PayoutsOptions { Uri = TestUri, SigningKey = new SigningKey { PrivateKey = "xxx" } }.Validate());
        }

        [Fact]
        public void Invalid_if_no_private_key()
        {
            Assert.Throws<ValidationException>(() =>
                new PayoutsOptions { Uri = TestUri, SigningKey = new SigningKey { KeyId = "xxx" } }.Validate());
        }

        [Fact]
        public void Valid_if_signing_key_provided()
        {
            new PayoutsOptions { Uri = TestUri, SigningKey = new SigningKey { KeyId = "xxx", PrivateKey = "xxx" } }.Validate();
        }
    }
}
