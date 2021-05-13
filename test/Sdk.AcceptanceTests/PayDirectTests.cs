using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.PayDirect.Model;
using Xunit;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public class PayDirectTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        private readonly MockBankClient _mockBankClient = new();

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

            var depositResponse = await _fixture.Api.PayDirect.Deposit(request);
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

            DepositRequest depositRequest = CreateDepositRequest(userId, depositId);
            DepositResponse depositResponse = await _fixture.Api.PayDirect.Deposit(depositRequest);

            depositResponse.ShouldNotBeNull();

            Deposit deposit = await _fixture.Api.PayDirect.GetDeposit(userId, depositId);

            deposit.ShouldNotBeNull();
            deposit.DepositId.ShouldBe(depositId);
            deposit.Status.ShouldNotBeNullOrWhiteSpace();
            deposit.ProviderId.ShouldBe(depositRequest.Deposit.ProviderId);
            deposit.Currency.ShouldBe(depositRequest.Deposit.Currency);
            deposit.AmountInMinor.ShouldBe(depositRequest.Deposit.AmountInMinor);
        }

        [Fact]
        public async Task Can_get_user_accounts()
        {
            DepositRequest depositRequest = CreateDepositRequest();
            DepositResponse depositResponse = await _fixture.Api.PayDirect.Deposit(depositRequest);

            await _mockBankClient.Authorize(depositResponse.AuthFlow.Uri);

            await TestUtils.RepeatUntil(
                () => _fixture.Api.PayDirect.GetDeposit(depositRequest.UserId, depositRequest.Deposit.DepositId),
                deposit => deposit.Status == "settled",
                5,
                TimeSpan.FromSeconds(3)
            );

            IEnumerable<UserAcccount> accounts = await _fixture.Api.PayDirect.GetUserAcccounts(depositRequest.UserId);
            accounts.ShouldNotBeNull();
            accounts.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Can_perform_closed_loop_withdrawal()
        {
            DepositRequest depositRequest = CreateDepositRequest();
            DepositResponse depositResponse = await _fixture.Api.PayDirect.Deposit(depositRequest);

            // Authorize the deposit to ensure the user has an account
            await _mockBankClient.Authorize(depositResponse.AuthFlow.Uri);

            // Wait for the deposit to settle
            Deposit deposit = await TestUtils.RepeatUntil(
                () => _fixture.Api.PayDirect.GetDeposit(depositRequest.UserId, depositRequest.Deposit.DepositId),
                deposit => deposit.Status == "settled",
                5,
                TimeSpan.FromSeconds(3)
            );
            
            // Withdraw funds from the account
            WithdrawalResponse response = await _fixture.Api.PayDirect.Withdraw(new UserWithdrawalRequest(
                depositRequest.UserId,
                deposit.Settled.AccountId,
                "Test Payment",
                1,
                "GBP",
                Guid.NewGuid()
            ));

            response.ShouldNotBeNull();
        }

        [Fact]
        public async Task Can_perform_open_loop_withdrawal()
        {
            DepositRequest depositRequest = CreateDepositRequest();
            DepositResponse depositResponse = await _fixture.Api.PayDirect.Deposit(depositRequest);

            // Authorize the deposit to ensure the user has an account
            await _mockBankClient.Authorize(depositResponse.AuthFlow.Uri);

            // Wait for the deposit to settle
            Deposit deposit = await TestUtils.RepeatUntil(
                () => _fixture.Api.PayDirect.GetDeposit(depositRequest.UserId, depositRequest.Deposit.DepositId),
                deposit => deposit.Status == "settled",
                5,
                TimeSpan.FromSeconds(3)
            );

            UserAcccount account = (await _fixture.Api.PayDirect.GetUserAcccounts(depositRequest.UserId)).FirstOrDefault();
            
            // Withdraw funds from the account
            WithdrawalResponse response = await _fixture.Api.PayDirect.Withdraw(new WithdrawalRequest(
                account.Name,
                account.Iban,
                "Test Payment",
                1,
                "GBP",
                "withdrawal",
                Guid.NewGuid()
            ));

            response.ShouldNotBeNull();
        }

        private static DepositRequest CreateDepositRequest(Guid? userId = null, Guid? depositId = null)
        {
            return new(
                userId ?? Guid.NewGuid(),
                new DepositRequest.DepositRequestDetails(
                    amountInMinor: 100,
                    currency: "GBP",
                    providerId: "mock-payments-gb-redirect",
                    depositId: depositId
                )
                {
                    SchemeId = "faster_payments_service",
                    Remitter = new DepositRequest.ParticipantDetails(
                        new DepositRequest.AccountIdentifierDetails(type: "sort_code_account_number")
                        {
                            AccountNumber = "12345602",
                            SortCode = "500000",
                        }
                    )
                    {
                        Name = "A less lucky someone"
                    }
                },
                new DepositRequest.AuthFlowRequestDetails(type: "redirect")
                {
                    ReturnUri = "https://localhost:5001/home/callback"
                }
            );
        }
    }
}
