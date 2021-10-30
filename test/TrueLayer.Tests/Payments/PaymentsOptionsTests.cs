using System;
using TrueLayer.Payments;
using Xunit;

namespace TrueLayer.Tests.Payments
{
    public class PaymentsOptionsTests
    {
        private static Uri TestUri = new Uri("http://api.truelayer.com");

        [Fact]
        public void Invalid_if_no_signing_key()
        {
            Assert.Throws<ArgumentNullException>("SigningKey", () => new PaymentsOptions { Uri = TestUri }.Validate());
        }

        [Fact]
        public void Invalid_if_no_signing_key_id()
        {
            Assert.Throws<ArgumentException>("KeyId", () =>
                new PaymentsOptions { Uri = TestUri, SigningKey = new SigningKey { PrivateKey = "xxx" } }.Validate());
        }

        [Fact]
        public void Invalid_if_no_private_key()
        {
            Assert.Throws<ArgumentException>("PrivateKey", () =>
                new PaymentsOptions { Uri = TestUri, SigningKey = new SigningKey { KeyId = "xxx" } }.Validate());
        }

        [Fact]
        public void Valid_if_signing_key_provided()
        {
            new PaymentsOptions { Uri = TestUri, SigningKey = new SigningKey { KeyId = "xxx", PrivateKey = "xxx" } }.Validate();
        }
    }
}
