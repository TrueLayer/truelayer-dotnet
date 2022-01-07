using System;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    public class UsersTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public UsersTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_get_user()
        {
            CreatePaymentRequest paymentRequest = CreatePaymentRequest();

            var response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.IsSuccessful.ShouldBeTrue();
            var createPaymentUser = response.Data.AsT0.User;

            var getUserResponse
                = await _fixture.Client.Users.GetUser(createPaymentUser.Id);

            getUserResponse.IsSuccessful.ShouldBeTrue();

            getUserResponse.Data.ShouldNotBeNull();
            var user = getUserResponse.Data!;
            user.Email.ShouldBe(paymentRequest.User.AsT0.Email);
            user.Id.ShouldBe(createPaymentUser.Id);
            user.Name.ShouldBe(paymentRequest.User.AsT0.Name);
            user.Phone.ShouldBe(paymentRequest.User.AsT0.Phone);
        }

        private static CreatePaymentRequest CreatePaymentRequest()
            => new CreatePaymentRequest(
                100,
                Currencies.GBP,
                new PaymentMethod.BankTransfer
                {
                    ProviderFilter = new ProviderFilter
                    {
                        ProviderIds = new[] { "mock-payments-gb-redirect" }
                    }
                },
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SchemeIdentifier.SortCodeAccountNumber("567890", "12345678")
                ),
                new PaymentUserRequest.NewUser("Jane Doe", email: "jane.doe@example.com", phone: "+442079460087")
            );
    }
}
