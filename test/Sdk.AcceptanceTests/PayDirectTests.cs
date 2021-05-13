using System;
using System.Collections.Generic;
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
            IEnumerable<AccountBalance> accounts = await _fixture.Api.PayDirect.GetAccountBalances();
            accounts.ShouldNotBeNull();

            AccountBalance defaultAccount = accounts.First();
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
            var request = CreateDepositRequest();

            var depositResponse = await _fixture.Api.PayDirect.InitiateDeposit(request);
            depositResponse.ShouldNotBeNull();
            depositResponse.Deposit.ShouldNotBeNull();
            depositResponse.AuthFlow.ShouldNotBeNull();
            depositResponse.AuthFlow.Uri.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Can_retrieve_deposit_details()
        {
            Guid userId = Guid.NewGuid();
            Guid depositId = Guid.NewGuid();

            InitiateDepositRequest depositRequest = CreateDepositRequest(userId, depositId);
            InitiateDepositResponse depositResponse = await _fixture.Api.PayDirect.InitiateDeposit(depositRequest);

            depositResponse.ShouldNotBeNull();

            Deposit deposit = await _fixture.Api.PayDirect.GetDeposit(userId, depositId);

            deposit.ShouldNotBeNull();
            deposit.DepositId.ShouldBe(depositId);
            deposit.Status.ShouldNotBeNullOrWhiteSpace();
            deposit.ProviderId.ShouldBe(depositRequest.Deposit.ProviderId);
            deposit.Currency.ShouldBe(depositRequest.Deposit.Currency);
            deposit.AmountInMinor.ShouldBe(depositRequest.Deposit.AmountInMinor);
        }

        private static InitiateDepositRequest CreateDepositRequest(Guid? userId = null, Guid? depositId = null)
        {
            return new(
                userId ?? Guid.NewGuid(),
                new InitiateDepositRequest.DepositRequestDetails(
                    amountInMinor: 100,
                    currency: "GBP",
                    providerId: "ob-sandbox-natwest",
                    depositId: depositId
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
