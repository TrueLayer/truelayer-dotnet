using System;
using System.Net;
using System.Threading.Tasks;
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

            var response = await _fixture.TlClients[0].Payouts.CreatePayout(payoutRequest);

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data!.Id));
        }

        [Fact]
        public async Task Can_create_pln_payout()
        {
            CreatePayoutRequest payoutRequest = CreatePlnPayoutRequest();

            var response = await _fixture.TlClients[0].Payouts.CreatePayout(payoutRequest);

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data!.Id));
        }

        [Fact]
        public async Task Can_get_payout()
        {
            CreatePayoutRequest payoutRequest = CreatePayoutRequest();

            var response = await _fixture.TlClients[0].Payouts.CreatePayout(
                payoutRequest, idempotencyKey: Guid.NewGuid().ToString());

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data!.Id));

            var getPayoutResponse = await _fixture.TlClients[0].Payouts.GetPayout(response.Data.Id);

            Assert.Equal(HttpStatusCode.OK, getPayoutResponse.StatusCode);
            Assert.NotNull(getPayoutResponse.Data.Value);
            PayoutDetails? details = getPayoutResponse.Data.Value as PayoutDetails;

            Assert.NotNull(details);
            Assert.Equal(response.Data.Id, details!.Id);
            Assert.Equal(payoutRequest.Currency, details.Currency);
            Assert.NotNull(details.Beneficiary.AsT1);
            Assert.Contains(details.Status, new[] { "pending", "authorized", "executed", "failed" });
            Assert.NotEqual(DateTime.MinValue, details.CreatedAt);
            Assert.NotEqual(DateTime.MaxValue, details.CreatedAt);
            Assert.Equal(payoutRequest.Metadata!.Count, details.Metadata!.Count);
            foreach (var kvp in payoutRequest.Metadata!)
            {
                Assert.True(details.Metadata!.ContainsKey(kvp.Key));
                Assert.Equal(kvp.Value, details.Metadata![kvp.Key]);
            }
        }

        [Fact]
        public async Task GetPayout_Url_As_PayoutId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string payoutId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Payouts.GetPayout(payoutId));
            Assert.Equal("Value is malformed (Parameter 'id')", result.Message);
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
                metadata: new() { { "a", "b" } },
                schemeSelection: new SchemeSelection.InstantOnly()
            );

        private static CreatePayoutRequest CreatePlnPayoutRequest()
            => new(
                "fdb6007b-78c0-dbc0-60dd-d4c6f6908e3b", //pln merchant account
                100,
                Currencies.PLN,
                new Beneficiary.ExternalAccount(
                    "Ms. Lucky",
                    "truelayer-dotnet",
                    new AccountIdentifier.Iban("GB25CLRB04066800046876"),
                    dateOfBirth: new DateTime(1970, 12, 31),
                    address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
                metadata: new() { { "a", "b" } },
                schemeSelection: new SchemeSelection.InstantOnly()
            );
    }
}
