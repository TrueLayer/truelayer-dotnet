using System;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Tests.Payments
{
    public class PaymentUserTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData(default(string?), default(string?))]
        [InlineData(" ", " ")]
        public void New_user_throws_if_user_and_email_not_provided(string? email, string? phone)
        {
            Assert.Throws<ArgumentException>(() => new PaymentUser.NewUser("name", email: email, phone: phone));
        }

        [Theory]
        [InlineData("a@b.com", default(string?))]
        [InlineData(default(string?), "1234567890")]
        public void Can_create_new_user_with_email_or_phone(string? email, string? phone)
        {
            _ = new PaymentUser.NewUser("name", email: email, phone: phone);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_throws_if_email_empty_or_whitespace(string? email)
        {
            Assert.Throws<ArgumentException>("email", () => new PaymentUser.NewUser("name", email: email, phone: "01234567890"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_throws_if_phone_empty_or_whitespace(string? phone)
        {
            Assert.Throws<ArgumentException>("phone", () => new PaymentUser.NewUser("name", email: "a@b.com", phone: phone));
        }
    }
}
