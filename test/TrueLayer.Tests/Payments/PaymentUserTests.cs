using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
