using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TrueLayer.Common;
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

            var response = await _fixture.TlClients[0].Payouts.CreatePayout(
                payoutRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Can_get_payout()
        {
            CreatePayoutRequest payoutRequest = CreatePayoutRequest();

            var response = await _fixture.TlClients[0].Payouts.CreatePayout(
                payoutRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().NotBeNullOrWhiteSpace();

            var getPayoutResponse = await _fixture.TlClients[0].Payouts.GetPayout(response.Data.Id);

            getPayoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getPayoutResponse.Data.Value.Should().NotBeNull();
            PayoutDetails? details = getPayoutResponse.Data.Value as PayoutDetails;

            details.Should().NotBeNull();
            details!.Id.Should().Be(response.Data.Id);
            details.Currency.Should().Be(payoutRequest.Currency);
            details.Beneficiary.AsT1.Should().NotBeNull();
            details.Status.Should().BeOneOf("pending", "authorized", "executed", "failed");
            details.CreatedAt.Should().NotBe(DateTime.MinValue);
            details.CreatedAt.Should().NotBe(DateTime.MaxValue);
            details.Metadata.Should().BeEquivalentTo(payoutRequest.Metadata);
        }

        private CreatePayoutRequest CreatePayoutRequest()
            => new(
                _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId,
                100,
                Currencies.GBP,
                new Beneficiary.ExternalAccount(
                    "Ms. Lucky",
                    "truelayer-dotnet",
                    new AccountIdentifier.Iban("GB33BUKB20201555555555"),
                    dateOfBirth: new DateTime(1970, 12, 31),
                    address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
                metadata: new() { { "a", "b" } }
            );

    }
}
