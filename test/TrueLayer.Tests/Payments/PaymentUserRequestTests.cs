using System;
using Shouldly;
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
            Assert.Throws<ArgumentException>("name", () => new PaymentUserRequest(name, null, null, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_throws_if_email_empty_or_whitespace(string email)
        {
            Assert.Throws<ArgumentException>("email", () => new PaymentUserRequest(null, email, null, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_throws_if_phone_empty_or_whitespace(string phone)
        {
            Assert.Throws<ArgumentException>("phone", () => new PaymentUserRequest(null, null, phone, null, null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_id_null_or_whitespace(string? id)
        {
            Assert.Throws<ArgumentException>("id", () => new PaymentUserRequest(id!, null, null, null, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_name_empty_or_whitespace(string name)
        {
            Assert.Throws<ArgumentException>("name", () => new PaymentUserRequest("id", name, null, null, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_email_empty_or_whitespace(string email)
        {
            Assert.Throws<ArgumentException>("email", () => new PaymentUserRequest("id", null, email, null, null, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void New_user_with_id_throws_if_phone_empty_or_whitespace(string phone)
        {
            Assert.Throws<ArgumentException>("phone", () => new PaymentUserRequest("id", null, null, phone, null, null));
        }

        [Fact]
        public void New_user_with_date_of_birth_gets_only_date_part_of_it()
        {
            // Arrange
            var dob = new DateTime(1969, 12, 28, 12, 33, 55);
            var user = new PaymentUserRequest(id: "id", dateOfBirth: dob);
            
            user.DateOfBirth.HasValue.ShouldBeTrue();
            user.DateOfBirth!.Value.ShouldBeEquivalentTo(new DateTime(1969, 12, 28));
            user.DateOfBirth!.Value.TimeOfDay.ShouldBeEquivalentTo(TimeSpan.Zero);
        }
    }
}
