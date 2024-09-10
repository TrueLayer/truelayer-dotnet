using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Common;
using TrueLayer.MerchantAccounts.Model;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    public class MerchantTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public MerchantTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_get_merchant_accounts()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource(5000).Token;

            // Act
            var response = await _fixture.Client.MerchantAccounts.ListMerchantAccounts(cancellationToken);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {response.TraceId}");
            response.Data.ShouldNotBeNull();
            response.Data.Items.ShouldBeOfType<List<MerchantAccount>>();
        }

        [Fact]
        public async Task Can_get_specific_merchant_account()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource(5000).Token;

            (string merchantId, string? traceId) = await GetMerchantAccountId(cancellationToken);

            // Act
            var merchantResponse = await _fixture.Client.MerchantAccounts.GetMerchantAccount(merchantId, cancellationToken);

            // Assert
            merchantResponse.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {traceId}");
            merchantResponse.Data.ShouldNotBeNull();
            merchantResponse.Data.Id.ShouldBe(merchantId);
            merchantResponse.Data.AccountHolderName.ShouldNotBeNullOrWhiteSpace();
            merchantResponse.Data.Currency.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Can_get_merchant_account_transactions()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            (string merchantId, string? traceId) = await GetMerchantAccountId(cancellationToken);

            // Act
            var merchantResponse = await _fixture.Client.MerchantAccounts.GetTransactions(
                merchantId,
                DateTimeOffset.UtcNow.AddYears(-1),
                DateTimeOffset.UtcNow,
                cancellationToken: CancellationToken.None);

            // Assert
            merchantResponse.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {traceId}");
            merchantResponse.Data.ShouldNotBeNull();
            merchantResponse.Data.Items.ShouldNotBeEmpty();
            foreach (var item in merchantResponse.Data.Items)
            {
                item.Match(
                    payment => AssertTransaction(payment),
                    externalPayment => AssertTransaction(externalPayment),
                    pendingPayout => AssertTransaction(pendingPayout),
                    executedPayout => AssertTransaction(executedPayout),
                    refund => AssertTransaction(refund));
            }

            merchantResponse.Data.Pagination?.NextCursor.ShouldNotBeNullOrWhiteSpace();
        }

        private static void AssertBaseTransaction(MerchantAccountTransactions.BaseTransaction baseTransaction,string[] expectedStatuses)
        {
            baseTransaction.ShouldNotBeNull();
            baseTransaction.Id.ShouldNotBeNullOrWhiteSpace();
            baseTransaction.Currency.ShouldNotBeNullOrWhiteSpace();
            baseTransaction.AmountInMinor.ShouldBeGreaterThan(0);
            baseTransaction.Status.ShouldBeOneOf(expectedStatuses);
        }

        private static bool AssertTransaction(MerchantAccountTransactions.MerchantAccountPayment payment)
        {
            AssertBaseTransaction(payment, new [] { "settled" });
            payment.SettledAt.ShouldNotBeOneOf(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            payment.PaymentSource.ShouldNotBeNull();
            payment.PaymentId.ShouldNotBeNullOrWhiteSpace();
            return true;
        }

        private static bool AssertTransaction(MerchantAccountTransactions.ExternalPayment externalPayment)
        {
            AssertBaseTransaction(externalPayment, new []{ "settled" });
            externalPayment.SettledAt.ShouldNotBeOneOf(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            externalPayment.Remitter.ShouldNotBeNull();
            externalPayment.ReturnFor.Match(
                returnedBy =>
                {
                    returnedBy?.ReturnedId.ShouldNotBeNullOrWhiteSpace();
                    return true;
                },
                returnedTo => true);
            return true;
        }

        private static void AssertBaseTransactionPayout(MerchantAccountTransactions.BaseTransactionPayout baseTransaction)
        {
           AssertBaseTransaction(baseTransaction, new [] { "executed", "pending" });
            baseTransaction.CreatedAt.ShouldNotBeOneOf(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            baseTransaction.Beneficiary.Match(
                externalAccount =>
                {
                    externalAccount.Reference.ShouldNotBeNullOrWhiteSpace();
                    return true;
                },
                paymentSource =>
                {
                    paymentSource.Reference.ShouldNotBeNullOrWhiteSpace();
                    return true;
                },
                businessAccount =>
                {
                    businessAccount.Reference.ShouldNotBeNullOrWhiteSpace();
                    return true;
                },
                userDetermined =>
                {
                    userDetermined.Reference.ShouldNotBeNullOrWhiteSpace();
                    return true;
                });
            baseTransaction.ContextCode.ShouldNotBeNullOrWhiteSpace();
            baseTransaction.PayoutId.ShouldNotBeNullOrWhiteSpace();
        }

        private static bool AssertTransaction(MerchantAccountTransactions.PendingPayout pendingPayout)
        {
            AssertBaseTransactionPayout(pendingPayout);
            return true;
        }

        private static bool AssertTransaction(MerchantAccountTransactions.ExecutedPayout executedPayout)
        {
            AssertBaseTransactionPayout(executedPayout);
            return true;
        }

        private static bool AssertTransaction(MerchantAccountTransactions.Refund refund)
        {
            AssertBaseTransaction(refund, new []{ "executed", "pending" });
            refund.PaymentId.ShouldNotBeNullOrWhiteSpace();
            refund.RefundId.ShouldNotBeNull();
            return true;
        }

        private async Task<(string merchantId, string? traceId)> GetMerchantAccountId(CancellationToken cancellationToken)
        {
            var listMerchants = await _fixture.Client.MerchantAccounts.ListMerchantAccounts(cancellationToken);
            listMerchants.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {listMerchants.TraceId}");
            listMerchants.Data.ShouldNotBeNull();
            listMerchants.Data.Items.ShouldNotBeEmpty();
            var merchantId = listMerchants.Data.Items.First().Id;
            return (merchantId, listMerchants.TraceId);
        }

        [Fact]
        public async Task Can_get_payment_sources()
        {
            // Arrange
            var listMerchants = await _fixture.Client.MerchantAccounts.ListMerchantAccounts();
            listMerchants.Data.ShouldNotBeNull();
            listMerchants.Data.Items.ShouldNotBeEmpty();
            var merchantId = listMerchants.Data!.Items.First(x => x.Currency == "GBP").Id;

            CreatePaymentRequest paymentRequest = CreatePaymentRequest(merchantId);

            var createPaymentResponse = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

            createPaymentResponse.IsSuccessful.ShouldBeTrue();
            var createPaymentUser = createPaymentResponse.Data!.AsT0.User;

            // Act
            var getPaymentSourcesResponse
                = await _fixture.Client.MerchantAccounts.GetPaymentSources(merchantId, createPaymentUser.Id);

            // Assert
            getPaymentSourcesResponse.IsSuccessful.ShouldBeTrue();
            getPaymentSourcesResponse.Data.ShouldNotBeNull();
            var responseBody = getPaymentSourcesResponse.Data!;
            responseBody.Items.ShouldBeEmpty();
        }

        private static CreatePaymentRequest CreatePaymentRequest(string merchantId)
            => new CreatePaymentRequest(
                100,
                Currencies.GBP,
                new PaymentMethod.BankTransfer(
                    new Provider.UserSelected
                    {
                        Filter = new ProviderFilter { ProviderIds = new[] { "mock-payments-gb-redirect" } }
                    },
                    new Beneficiary.MerchantAccount(merchantId)
                    {
                        Reference = "Test payment",
                        Verification = new Verification.Automated
                        {
                            RemitterName = true,
                            RemitterDateOfBirth = false
                        }
                    }),
                new PaymentUserRequest(
                    name: "Jane Doe",
                    email: "jane.doe@example.com",
                    phone: "+442079460087",
                    dateOfBirth: new DateTime(1999, 1, 1),
                    address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
                null
            );
    }
}
