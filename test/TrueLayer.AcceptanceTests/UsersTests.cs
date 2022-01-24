using System;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payments.Model;
using TrueLayer.Users.Model;
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
        public async Task Can_get_user_external_accounts()
        {
            CreatePaymentRequest paymentRequest = CreatePaymentRequest();

            var response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.IsSuccessful.ShouldBeTrue();
            var createPaymentUser = response.Data!.User;

            var getUserExternalAccountsResponse
                = await _fixture.Client.Users.GetUserExternalAccounts(createPaymentUser.Id);

            getUserExternalAccountsResponse.IsSuccessful.ShouldBeTrue();
            getUserExternalAccountsResponse.Data.ShouldNotBeNull();
            var externalAccounts = getUserExternalAccountsResponse.Data!;
            externalAccounts.Items.ShouldBeOfType<UserExternalAccount[]>();
            externalAccounts.Items.ShouldBeEmpty();

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
                new PaymentUserRequest("Jane Doe", "jane.doe@example.com", "+442079460087")
            );
    }
}
