using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TrueLayer.Common;
using TrueLayer.MerchantAccounts.Model;
using TrueLayer.Payouts.Model;
using Xunit;
using static TrueLayer.Payouts.Model.GetPayoutsResponse;

namespace TrueLayer.AcceptanceTests
{
    public class PayoutTests : IClassFixture<ApiTestFixture>, IAsyncLifetime
    {
        private readonly ApiTestFixture _fixture;
        private MerchantAccount? _merchantAccount;

        public PayoutTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_create_payout()
        {
            CreatePayoutRequest payoutRequest = CreatePayoutRequest();

            var response = await _fixture.Client.Payouts.CreatePayout(payoutRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Can_get_payout()
        {
            CreatePayoutRequest payoutRequest = CreatePayoutRequest();

            var response = await _fixture.Client.Payouts.CreatePayout(
                payoutRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().NotBeNullOrWhiteSpace();

            var getPayoutResponse = await _fixture.Client.Payouts.GetPayout(response.Data.Id);

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

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            var accounts = await _fixture.Client.MerchantAccounts.ListMerchantAccounts();

            if (!accounts.IsSuccessful || !accounts.Data.Items.Any())
            {
                throw new InvalidOperationException("You must have a merchant account in order to perform a payout");
            }

            _merchantAccount = accounts.Data.Items.Single(x => x.Currency == Currencies.GBP);
        }

        private CreatePayoutRequest CreatePayoutRequest()
            => new(
                _merchantAccount!.Id,
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
