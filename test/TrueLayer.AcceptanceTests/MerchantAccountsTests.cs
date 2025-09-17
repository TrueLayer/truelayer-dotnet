using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
            // Act
            var response = await _fixture.TlClients[1].MerchantAccounts.ListMerchantAccounts();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.IsType<List<MerchantAccount>>(response.Data!.Items);
        }

        [Fact]
        public async Task Can_get_specific_merchant_account()
        {
            // Arrange
            var merchantId = _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId;

            // Act
            var merchantResponse = await _fixture.TlClients[0].MerchantAccounts.GetMerchantAccount(merchantId);

            // Assert
            Assert.Equal(HttpStatusCode.OK, merchantResponse.StatusCode);
            Assert.NotNull(merchantResponse.Data);
            Assert.Equal(merchantId, merchantResponse.Data!.Id);
            Assert.False(string.IsNullOrWhiteSpace(merchantResponse.Data.AccountHolderName));
            Assert.False(string.IsNullOrWhiteSpace(merchantResponse.Data.Currency));
        }

        [Fact]
        public async Task Can_get_merchant_account_transactions()
        {
            // Arrange
            var merchantId = _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId;

            // Act
            var merchantResponse = await _fixture.TlClients[0].MerchantAccounts.GetTransactions(
                merchantId,
                DateTimeOffset.UtcNow.AddDays(-7),
                DateTimeOffset.UtcNow,
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, merchantResponse.StatusCode);
            Assert.NotNull(merchantResponse.Data);
            Assert.NotEmpty(merchantResponse.Data!.Items);
            foreach (var item in merchantResponse.Data.Items)
            {
                item.Match(
                    payment => AssertTransaction(payment),
                    externalPayment => AssertTransaction(externalPayment),
                    pendingPayout => AssertTransaction(pendingPayout),
                    executedPayout => AssertTransaction(executedPayout),
                    refund => AssertTransaction(refund));
            }

            Assert.False(string.IsNullOrWhiteSpace(merchantResponse.Data.Pagination?.NextCursor));
        }

        private static void AssertBaseTransaction(MerchantAccountTransactions.BaseTransaction baseTransaction,string[] expectedStatuses)
        {
            Assert.NotNull(baseTransaction);
            Assert.False(string.IsNullOrWhiteSpace(baseTransaction.Id));
            Assert.False(string.IsNullOrWhiteSpace(baseTransaction.Currency));
            Assert.True(baseTransaction.AmountInMinor > 0);
            Assert.Contains(baseTransaction.Status, expectedStatuses);
        }

        private static bool AssertTransaction(MerchantAccountTransactions.MerchantAccountPayment payment)
        {
            AssertBaseTransaction(payment, ["settled"]);
            Assert.NotEqual(DateTimeOffset.MinValue, payment.SettledAt);
            Assert.NotEqual(DateTimeOffset.MaxValue, payment.SettledAt);
            Assert.NotNull(payment.PaymentSource);
            Assert.False(string.IsNullOrWhiteSpace(payment.PaymentId));
            return true;
        }

        private static bool AssertTransaction(MerchantAccountTransactions.ExternalPayment externalPayment)
        {
            AssertBaseTransaction(externalPayment, ["settled"]);
            Assert.NotEqual(DateTimeOffset.MinValue, externalPayment.SettledAt);
            Assert.NotEqual(DateTimeOffset.MaxValue, externalPayment.SettledAt);
            Assert.NotNull(externalPayment.Remitter);
            externalPayment.ReturnFor.Match(
                returnedBy =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(returnedBy?.ReturnedId));
                    return true;
                },
                returnedTo => true);
            return true;
        }

        private static void AssertBaseTransactionPayout(MerchantAccountTransactions.BaseTransactionPayout baseTransaction)
        {
           AssertBaseTransaction(baseTransaction, ["executed", "pending"]);
            Assert.NotEqual(DateTimeOffset.MinValue, baseTransaction.CreatedAt);
            Assert.NotEqual(DateTimeOffset.MaxValue, baseTransaction.CreatedAt);
            baseTransaction.Beneficiary.Match(
                externalAccount =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(externalAccount.Reference));
                    return true;
                },
                paymentSource =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(paymentSource.Reference));
                    return true;
                },
                businessAccount =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(businessAccount.Reference));
                    return true;
                },
                userDetermined =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(userDetermined.Reference));
                    return true;
                });
            Assert.False(string.IsNullOrWhiteSpace(baseTransaction.ContextCode));
            Assert.False(string.IsNullOrWhiteSpace(baseTransaction.PayoutId));
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
            AssertBaseTransaction(refund, ["executed", "pending"]);
            Assert.False(string.IsNullOrWhiteSpace(refund.PaymentId));
            Assert.NotNull(refund.RefundId);
            return true;
        }

        [Fact]
        public async Task Can_get_payment_sources()
        {
            // Arrange
            var merchantId = _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId;
            var paymentRequest = CreatePaymentRequest(merchantId);

            var createPaymentResponse = await _fixture.TlClients[0].Payments.CreatePayment(
                paymentRequest,
                idempotencyKey: Guid.NewGuid().ToString());

            Assert.True(createPaymentResponse.IsSuccessful);
            var createPaymentUser = createPaymentResponse.Data.AsT0.User;

            // Act
            var getPaymentSourcesResponse
                = await _fixture.TlClients[0].MerchantAccounts.GetPaymentSources(merchantId, createPaymentUser.Id);

            // Assert
            Assert.True(getPaymentSourcesResponse.IsSuccessful);
            Assert.NotNull(getPaymentSourcesResponse.Data);
            var responseBody = getPaymentSourcesResponse.Data!;
            Assert.Empty(responseBody.Items);
        }

        private static CreatePaymentRequest CreatePaymentRequest(string merchantId)
            => new(
                100,
                Currencies.GBP,
                new PaymentMethod.BankTransfer(
                    new Provider.UserSelected
                    {
                        Filter = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] }
                    },
                    new Beneficiary.MerchantAccount(merchantId)
                    {
                        Reference = "Test payment",
                        Verification = new Verification.Automated
                        {
                            RemitterName = true,
                            RemitterDateOfBirth = false
                        },
                        StatementReference = "Statement ref",
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
