using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.PayDirect.Model;
using Xunit;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public class PayDirectTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public PayDirectTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_retrieve_account_balances()
        {
            QueryResponse<AccountBalance> accounts = await _fixture.Api.PayDirect.GetAccountBalances();
            accounts.ShouldNotBeNull();
            accounts.Results.ShouldNotBeNull();

            AccountBalance defaultAccount = accounts.Results.First();
            defaultAccount.Currency.ShouldNotBeNullOrWhiteSpace();
            defaultAccount.Iban.ShouldNotBeNullOrWhiteSpace();
            defaultAccount.AccountOwner.ShouldNotBeNullOrWhiteSpace();
            defaultAccount.Status.ShouldBe("enabled");
            defaultAccount.Enabled.ShouldBeTrue();
            defaultAccount.CurrentBalanceInMinor.ShouldBeGreaterThan(0);
            defaultAccount.AvailableBalanceInMinor.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Can_initiate_deposit()
        {
            var request = CreateDepositRequest(Guid.NewGuid());

            var depositResponse = await _fixture.Api.PayDirect.InitiateDeposit(request);
            depositResponse.Result.ShouldNotBeNull();
            depositResponse.Result.Deposit.ShouldNotBeNull();
            depositResponse.Result.AuthFlow.ShouldNotBeNull();
            depositResponse.Result.AuthFlow.Uri.ShouldNotBeNullOrEmpty();
        }

        private static InitiateDepositRequest CreateDepositRequest(Guid userId)
        {
            return new(
                userId,
                new InitiateDepositRequest.DepositRequestDetails(
                    amountInMinor: 100,
                    currency: "GBP",
                    providerId: "ob-sandbox-natwest"
                )
                {
                    SchemeId = "faster_payments_service",
                    Remitter = new InitiateDepositRequest.ParticipantDetails(
                        new InitiateDepositRequest.AccountIdentifierDetails(type: "sort_code_account_number")
                        {
                            AccountNumber = "12345602",
                            SortCode = "500000",
                        }
                    )
                    {
                        Name = "A less lucky someone"
                    }
                },
                new InitiateDepositRequest.AuthFlowRequestDetails(type: "redirect")
                {
                    ReturnUri = "https://localhost:5001/home/callback"
                }
            );
        }
    }
}
