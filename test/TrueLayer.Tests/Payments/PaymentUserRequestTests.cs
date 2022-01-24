using System;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Tests.Payments
{
    public class PaymentUserRequestTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_throws_if_name_empty_or_whitespace(string name)
        {
            Assert.Throws<ArgumentException>("name", () => new PaymentUserRequest(name, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_throws_if_email_empty_or_whitespace(string email)
        {
            Assert.Throws<ArgumentException>("email", () => new PaymentUserRequest(null, email, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_throws_if_phone_empty_or_whitespace(string phone)
        {
            Assert.Throws<ArgumentException>("phone", () => new PaymentUserRequest(null, null, phone));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_id_null_or_whitespace(string? id)
        {
            Assert.Throws<ArgumentException>("id", () => new PaymentUserRequest(id!, null, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_name_empty_or_whitespace(string name)
        {
            Assert.Throws<ArgumentException>("name", () => new PaymentUserRequest("id", name, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_email_empty_or_whitespace(string email)
        {
            Assert.Throws<ArgumentException>("email", () => new PaymentUserRequest("id", null, email, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_phone_empty_or_whitespace(string phone)
        {
            Assert.Throws<ArgumentException>("phone", () => new PaymentUserRequest("id", null, null, phone));
        }
    }
}
