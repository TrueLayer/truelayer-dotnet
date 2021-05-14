using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.PayDirect.Model;
using Xunit;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public class PayDirectTests : IClassFixture<ApiTestFixture>, IAsyncLifetime
    {
        private readonly ApiTestFixture _fixture;
        private readonly MockBankClient _mockBankClient = new();

        private Guid _depositId;
        private Guid _accountId;
        private Guid _userId; 

        public PayDirectTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            DepositRequest depositRequest = CreateDepositRequest();
            DepositResponse depositResponse = await _fixture.Api.PayDirect.Deposit(depositRequest);

            _userId = depositRequest.UserId;
            _depositId = depositRequest.Deposit.DepositId;

            await _mockBankClient.Authorize(depositResponse.AuthFlow.Uri);

            Deposit deposit = await TestUtils.RepeatUntil(
                () => _fixture.Api.PayDirect.GetDeposit(depositRequest.UserId, depositRequest.Deposit.DepositId),
                deposit => deposit.IsSettled,
                5,
                TimeSpan.FromSeconds(3)
            );

            deposit.ShouldNotBeNull();
            deposit.IsSettled.ShouldBeTrue();

            _accountId = deposit.Settled.AccountId;
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
            IEnumerable<UserAcccount> accounts = await _fixture.Api.PayDirect.GetUserAcccounts(_userId);
            accounts.ShouldNotBeNull();
            accounts.ShouldNotBeEmpty();

            UserAcccount defaultAccount = accounts.FirstOrDefault();
            defaultAccount.AccountId.ShouldNotBe(Guid.Empty);
            defaultAccount.Iban.ShouldNotBeNullOrWhiteSpace();
            defaultAccount.Name.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Can_perform_closed_loop_withdrawal()
        {
            // Withdraw funds from the account
            await _fixture.Api.PayDirect.Withdraw(new UserWithdrawalRequest(
                _userId,
                _accountId,
                "Test Payment",
                1,
                "GBP",
                Guid.NewGuid()
            ));
        }

        [Fact]
        public async Task Can_perform_open_loop_withdrawal()
        {
            UserAcccount account = (await _fixture.Api.PayDirect.GetUserAcccounts(_userId)).FirstOrDefault();
            
            // Withdraw funds from the account
            await _fixture.Api.PayDirect.Withdraw(new WithdrawalRequest(
                account.Name,
                account.Iban,
                "Test Payment",
                1,
                "GBP",
                ContextCodes.Withdrawal,
                Guid.NewGuid()
            ));
        }

        public Task DisposeAsync()
        {
            _mockBankClient.Dispose();
            return Task.CompletedTask;
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
