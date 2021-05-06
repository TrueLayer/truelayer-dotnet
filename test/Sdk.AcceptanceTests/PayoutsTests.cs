using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payouts.Model;
using Xunit;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public class PayoutsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public PayoutsTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_retrieve_account_balances()
        {
            QueryResponse<AccountBalance> accounts = await _fixture.Api.Payouts.GetAccountBalances();
            accounts.ShouldNotBeNull();
            accounts.Results.ShouldNotBeNull();

            AccountBalance defaultAccount = accounts.Results.First();
            defaultAccount.Currency.ShouldNotBeNullOrWhiteSpace();
            defaultAccount.Iban.ShouldNotBeNullOrWhiteSpace();
            defaultAccount.Status.ShouldBe("enabled");
            defaultAccount.Enabled.ShouldBeTrue();
            defaultAccount.CurrentBalanceInMinor.ShouldBeGreaterThan(0);
            defaultAccount.AvailableBalanceInMinor.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Can_validate_signing_key()
        {
            try
            {
                await _fixture.Api.Payouts.ValidateSigningKey();
            }
            catch (TrueLayerApiException apiex)
            {   

            }
        }

        [Fact]
        public async Task Can_initiate_payout()
        {
            var payoutRequest = new InitiatePayoutRequest(
                1,
                "GBP",
                "Ben Foster",
                "GB17CLRB04066800000072",
                "Good graces",
                "withdrawal"
            );

            await _fixture.Api.Payouts.InitiatePayout(payoutRequest);
        }
    }
}
