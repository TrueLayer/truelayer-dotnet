using System;
using System.Net;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payouts.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    public class PayoutTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public PayoutTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_create_payout()
        {
            CreatePayoutRequest payoutRequest = CreatePayoutRequest();

            var response = await _fixture.Client.Payouts.CreatePayout(
                payoutRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldNotBeNullOrWhiteSpace();
        }

        private static CreatePayoutRequest CreatePayoutRequest()
            => new CreatePayoutRequest(
                "<CHANGE_ME_WITH_VALID_MERCHANT_ID_FROM_YOUR_ACCOUNT>",
                100,
                Currencies.GBP,
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SchemeIdentifier.Iban("<CHANGE_ME_WITH_VALID_IBAN_FROM_YOUR_ACCOUNT>")
                )
            );
    }
}