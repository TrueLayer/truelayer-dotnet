using System;
using System.Net;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payouts.Model;
using Xunit;
using static TrueLayer.Payouts.Model.GetPayoutsResponse;

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

        [Fact]
        public async Task Can_get_payout()
        {
            CreatePayoutRequest payoutRequest = CreatePayoutRequest();

            var response = await _fixture.Client.Payouts.CreatePayout(
                payoutRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldNotBeNullOrWhiteSpace();

            var getPayoutResponse = await _fixture.Client.Payouts.GetPayout(response.Data.Id);

            getPayoutResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            getPayoutResponse.Data.Value.ShouldNotBeNull();
            PayoutDetails? details = getPayoutResponse.Data.Value as PayoutDetails;

            details.ShouldNotBeNull();
            details.Id.ShouldBe(response.Data.Id);
            details.Currency.ShouldBe(payoutRequest.Currency);
            details.Status.ShouldBeOneOf("pending", "authorized", "successful", "failed");
            details.CreatedAt.ShouldNotBeOneOf(DateTime.MinValue, DateTime.MaxValue);
        }

        private static CreatePayoutRequest CreatePayoutRequest()
            => new CreatePayoutRequest(
                "27e05025-407a-4b81-be84-1cea52a5125e",
                100,
                Currencies.GBP,
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SchemeIdentifier.Iban("GB98CLRB04066200005308")
                )
            );
    }
}
