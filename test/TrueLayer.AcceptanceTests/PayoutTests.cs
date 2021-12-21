using System;
using System.Configuration;
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
        private readonly string _merchantAccountId;
        private readonly string _iban;

        public PayoutTests(ApiTestFixture fixture)
        {
            const string merchantAccountIdEnvVarName = "TrueLayer__AcceptanceTests__MerchantAccountId";
            const string ibanEnvVarName = "TrueLayer__AcceptanceTests__Iban";

            _fixture = fixture;
            _merchantAccountId = Environment.GetEnvironmentVariable(merchantAccountIdEnvVarName)
#if DEBUG
                                 ?? "CHANGE ME WITH YOUR MERCHANT ID";
#else
                                 ?? throw new ConfigurationErrorsException($"NULL ${merchantAccountIdEnvVarName} environment variable");
#endif
            _iban = Environment.GetEnvironmentVariable(ibanEnvVarName)
#if DEBUG
                    ?? "CHANE ME WITH YOUR TEST IBAN";
#else
                    ?? throw new ConfigurationErrorsException($"NULL ${ibanEnvVarName} environment variable");
#endif


            Console.WriteLine(_merchantAccountId);
            Console.WriteLine(_iban);
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

        private CreatePayoutRequest CreatePayoutRequest()
            => new CreatePayoutRequest(
                _merchantAccountId,
                100,
                Currencies.GBP,
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SchemeIdentifier.Iban(_iban)
                )
            );
    }
}
