using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using TrueLayer.Common;
using TrueLayer.MerchantAccounts.Model;
using TrueLayer.Payments.Model;
using Xunit;
using static TrueLayer.Payments.Model.CreateProvider;
using static TrueLayer.Payments.Model.CreatePaymentMethod;

namespace TrueLayer.AcceptanceTests;

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
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull();
        response.Data!.Items.Should().BeOfType<List<MerchantAccount>>();
    }

    [Fact]
    public async Task Can_get_specific_merchant_account()
    {
        // Arrange
        var merchantId = _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId;

        // Act
        var merchantResponse = await _fixture.TlClients[0].MerchantAccounts.GetMerchantAccount(merchantId);

        // Assert
        merchantResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        merchantResponse.Data.Should().NotBeNull();
        merchantResponse.Data!.Id.Should().Be(merchantId);
        merchantResponse.Data.AccountHolderName.Should().NotBeNullOrWhiteSpace();
        merchantResponse.Data.Currency.Should().NotBeNullOrWhiteSpace();
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
        merchantResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        merchantResponse.Data.Should().NotBeNull();
        merchantResponse.Data!.Items.Should().NotBeEmpty();
        foreach (var item in merchantResponse.Data.Items)
        {
            item.Match(
                payment => AssertTransaction(payment),
                externalPayment => AssertTransaction(externalPayment),
                pendingPayout => AssertTransaction(pendingPayout),
                executedPayout => AssertTransaction(executedPayout),
                refund => AssertTransaction(refund));
        }

        merchantResponse.Data.Pagination?.NextCursor.Should().NotBeNullOrWhiteSpace();
    }

    private static void AssertBaseTransaction(MerchantAccountTransactions.BaseTransaction baseTransaction,string[] expectedStatuses)
    {
        baseTransaction.Should().NotBeNull();
        baseTransaction.Id.Should().NotBeNullOrWhiteSpace();
        baseTransaction.Currency.Should().NotBeNullOrWhiteSpace();
        baseTransaction.AmountInMinor.Should().BeGreaterThan(0);
        baseTransaction.Status.Should().BeOneOf(expectedStatuses);
    }

    private static bool AssertTransaction(MerchantAccountTransactions.MerchantAccountPayment payment)
    {
        AssertBaseTransaction(payment, ["settled"]);
        payment.SettledAt.Should().NotBe(DateTimeOffset.MinValue);
        payment.SettledAt.Should().NotBe(DateTimeOffset.MaxValue);
        payment.PaymentSource.Should().NotBeNull();
        payment.PaymentId.Should().NotBeNullOrWhiteSpace();
        return true;
    }

    private static bool AssertTransaction(MerchantAccountTransactions.ExternalPayment externalPayment)
    {
        AssertBaseTransaction(externalPayment, ["settled"]);
        externalPayment.SettledAt.Should().NotBe(DateTimeOffset.MinValue);
        externalPayment.SettledAt.Should().NotBe(DateTimeOffset.MaxValue);
        externalPayment.Remitter.Should().NotBeNull();
        externalPayment.ReturnFor.Match(
            returnedBy =>
            {
                returnedBy?.ReturnedId.Should().NotBeNullOrWhiteSpace();
                return true;
            },
            returnedTo => true);
        return true;
    }

    private static void AssertBaseTransactionPayout(MerchantAccountTransactions.BaseTransactionPayout baseTransaction)
    {
        AssertBaseTransaction(baseTransaction, ["executed", "pending"]);
        baseTransaction.CreatedAt.Should().NotBe(DateTimeOffset.MinValue);
        baseTransaction.CreatedAt.Should().NotBe(DateTimeOffset.MaxValue);
        baseTransaction.Beneficiary.Match(
            externalAccount =>
            {
                externalAccount.Reference.Should().NotBeNullOrWhiteSpace();
                return true;
            },
            paymentSource =>
            {
                paymentSource.Reference.Should().NotBeNullOrWhiteSpace();
                return true;
            },
            businessAccount =>
            {
                businessAccount.Reference.Should().NotBeNullOrWhiteSpace();
                return true;
            },
            userDetermined =>
            {
                userDetermined.Reference.Should().NotBeNullOrWhiteSpace();
                return true;
            });
        baseTransaction.ContextCode.Should().NotBeNullOrWhiteSpace();
        baseTransaction.PayoutId.Should().NotBeNullOrWhiteSpace();
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
        refund.PaymentId.Should().NotBeNullOrWhiteSpace();
        refund.RefundId.Should().NotBeNull();
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

        createPaymentResponse.IsSuccessful.Should().BeTrue();
        var createPaymentUser = createPaymentResponse.Data.AsT0.User;

        // Act
        var getPaymentSourcesResponse
            = await _fixture.TlClients[0].MerchantAccounts.GetPaymentSources(merchantId, createPaymentUser.Id);

        // Assert
        getPaymentSourcesResponse.IsSuccessful.Should().BeTrue();
        getPaymentSourcesResponse.Data.Should().NotBeNull();
        var responseBody = getPaymentSourcesResponse.Data!;
        responseBody.Items.Should().BeEmpty();
    }

    private static CreatePaymentRequest CreatePaymentRequest(string merchantId)
        => new(
            100,
            Currencies.GBP,
            new BankTransfer(
                new UserSelected
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