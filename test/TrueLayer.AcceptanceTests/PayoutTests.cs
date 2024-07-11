using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Shouldly;
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
            details.Status.ShouldBeOneOf("pending", "authorized", "executed", "failed");
            details.CreatedAt.ShouldNotBeOneOf(DateTime.MinValue, DateTime.MaxValue);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            var accounts = await _fixture.Client.MerchantAccounts.ListMerchantAccounts();

            if (!accounts.IsSuccessful || !accounts.Data.Items.Any())
            {
                throw new InvalidOperationException("You must have a merchant account in order to perform a payout");
            }

            _merchantAccount = accounts.Data.Items.Single(x => x.Currency == "GBP");
        }

        private CreatePayoutRequest CreatePayoutRequest()
            => new CreatePayoutRequest(
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
