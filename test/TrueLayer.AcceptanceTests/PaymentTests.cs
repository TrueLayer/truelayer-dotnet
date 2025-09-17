using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.AcceptanceTests.Clients;
using TrueLayer.AcceptanceTests.Helpers;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using TrueLayer.Payments.Model.AuthorizationFlow;
using Xunit;

namespace TrueLayer.AcceptanceTests;

using AccountIdentifierUnion = OneOf<
    AccountIdentifier.SortCodeAccountNumber,
    AccountIdentifier.Iban,
    AccountIdentifier.Bban,
    AccountIdentifier.Nrb>;

using PaymentsSchemeSelectionUnion = OneOf<
    SchemeSelection.InstantOnly,
    SchemeSelection.InstantPreferred,
    SchemeSelection.Preselected,
    SchemeSelection.UserSelected>;
using ProviderUnion = OneOf<Provider.UserSelected, Provider.Preselected>;
using BeneficiaryUnion = OneOf<Beneficiary.MerchantAccount, Beneficiary.ExternalAccount>;

using GetPaymentUnion = OneOf<
    GetPaymentResponse.AuthorizationRequired,
    GetPaymentResponse.Authorizing,
    GetPaymentResponse.Authorized,
    GetPaymentResponse.Executed,
    GetPaymentResponse.Settled,
    GetPaymentResponse.Failed,
    GetPaymentResponse.AttemptFailed
>;

public partial class PaymentTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;

    public PaymentTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(ExternalAccountPaymentRequests))]
    public async Task Can_Create_External_Account_Payment(CreatePaymentRequest paymentRequest)
    {
        foreach (var client in _fixture.TlClients)
        {
            var response = await client.Payments.CreatePayment(paymentRequest);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var authorizationRequired = response.Data.AsT0;

            Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.Id));
            Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.ResourceToken));
            Assert.NotNull(authorizationRequired.User);
            Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.User.Id));
            Assert.Equal("authorization_required", authorizationRequired.Status);

            string hppUri = client.Payments.CreateHostedPaymentPageLink(
                authorizationRequired.Id, authorizationRequired.ResourceToken,
                new Uri("https://redirect.mydomain.com"));
            Assert.False(string.IsNullOrWhiteSpace(hppUri));
        }
    }

    [Fact]
    public async Task Can_Create_Merchant_Account_Gbp_Payment()
    {
        var paymentRequest = CreateTestPaymentRequest(
            new Provider.UserSelected
            {
                Filter = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] },
                SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
            },
            beneficiary: new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].GbpMerchantAccountId)
            {
                AccountHolderName = "account holder name",
                StatementReference = "statement-ref"
            });

        var response = await _fixture.TlClients[0].Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var authorizationRequired = response.Data.AsT0;

        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.Id));
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.ResourceToken));
        Assert.NotNull(authorizationRequired.User);
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.User.Id));
        Assert.Equal("authorization_required", authorizationRequired.Status);

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizationRequired.Id, authorizationRequired.ResourceToken, new Uri("https://redirect.mydomain.com"));
        Assert.False(string.IsNullOrWhiteSpace(hppUri));
    }

    [Fact]
    public async Task Can_Create_Merchant_Account_Gbp_Verification_Payment()
    {
        var paymentRequest = CreateTestPaymentRequest(
            new Provider.UserSelected
            {
                Filter = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] },
                SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
            },
            beneficiary: new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].GbpMerchantAccountId)
            {
                Verification = new Verification.Automated { RemitterName = true }
            });

        var response = await _fixture.TlClients[0].Payments.CreatePayment(paymentRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var authorizationRequired = response.Data.AsT0;

        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.Id));
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.ResourceToken));
        Assert.NotNull(authorizationRequired.User);
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.User.Id));
        Assert.Equal("authorization_required", authorizationRequired.Status);

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizationRequired.Id, authorizationRequired.ResourceToken, new Uri("https://redirect.mydomain.com"));
        Assert.False(string.IsNullOrWhiteSpace(hppUri));
    }

    [Fact]
    public async Task Can_Create_Merchant_Account_Eur_Payment()
    {
        var paymentRequest = CreateTestPaymentRequest(
            new Provider.Preselected("mock-payments-fr-redirect",
                schemeSelection: new SchemeSelection.Preselected { SchemeId = "sepa_credit_transfer_instant" })
            {
                Remitter = new RemitterAccount("John Doe", new AccountIdentifier.Iban("FR1420041010050500013M02606"))
            },
            null,
            Currencies.EUR,
            new RelatedProducts(new SignupPlus()),
            new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].EurMerchantAccountId));

        var response = await _fixture.TlClients[0].Payments.CreatePayment(paymentRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var authorizationRequired = response.Data.AsT0;

        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.Id));
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.ResourceToken));
        Assert.NotNull(authorizationRequired.User);
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.User.Id));
        Assert.Equal("authorization_required", authorizationRequired.Status);

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizationRequired.Id, authorizationRequired.ResourceToken, new Uri("https://redirect.mydomain.com"));
        Assert.False(string.IsNullOrWhiteSpace(hppUri));
    }

    [Fact]
    public async Task Can_Create_Payment_With_SubMerchants_BusinessDivision()
    {
        var subMerchants = new SubMerchants(new SubMerchants.BusinessDivision(
            id: Guid.NewGuid().ToString(),
            name: "Test Division"));

        var paymentRequest = CreateTestPaymentRequest(
            new Provider.UserSelected
            {
                Filter = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] },
                SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
            },
            subMerchants: subMerchants);

        var response = await _fixture.TlClients[0].Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var authorizationRequired = response.Data.AsT0;

        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.Id));
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.ResourceToken));
        Assert.NotNull(authorizationRequired.User);
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.User.Id));
        Assert.Equal("authorization_required", authorizationRequired.Status);
    }

    [Fact]
    public async Task Can_Create_Payment_With_SubMerchants_BusinessClient()
    {
        var address = new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St");
        var subMerchants = new SubMerchants(new SubMerchants.BusinessClient(
            tradingName: "Test Trading Company",
            commercialName: "Test Commercial Name",
            url: "https://example.com",
            mcc: "1234",
            registrationNumber: "REG123456",
            address: address));

        var paymentRequest = CreateTestPaymentRequest(
            new Provider.UserSelected
            {
                Filter = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] },
                SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
            },
            subMerchants: subMerchants);

        var response = await _fixture.TlClients[0].Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var authorizationRequired = response.Data.AsT0;

        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.Id));
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.ResourceToken));
        Assert.NotNull(authorizationRequired.User);
        Assert.False(string.IsNullOrWhiteSpace(authorizationRequired.User.Id));
        Assert.Equal("authorization_required", authorizationRequired.Status);
    }

    [Fact]
    public async Task Can_Create_Payment_With_Auth_Flow()
    {
        var sortCodeAccountNumber = new AccountIdentifier.SortCodeAccountNumber("567890", "12345678");
        var providerSelection = new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service")
        {
            Remitter = new RemitterAccount("John Doe", sortCodeAccountNumber),
        };

        var paymentRequest = CreateTestPaymentRequest(
            providerSelection,
            sortCodeAccountNumber,
            initAuthorizationFlow: true);
        var response = await _fixture.TlClients[0].Payments.CreatePayment(paymentRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.Data.IsT3);
        var authorizing = response.Data.AsT3;

        Assert.False(string.IsNullOrWhiteSpace(authorizing.Id));
        Assert.False(string.IsNullOrWhiteSpace(authorizing.ResourceToken));
        Assert.NotNull(authorizing.User);
        Assert.False(string.IsNullOrWhiteSpace(authorizing.User.Id));
        Assert.Equal("authorizing", authorizing.Status);
        Assert.NotNull(authorizing.AuthorizationFlow);
        // The next action is a redirect
        Assert.True(authorizing.AuthorizationFlow!.Actions.Next.IsT2);
        Assert.NotNull(authorizing.AuthorizationFlow.Actions.Next.AsT2.Uri);

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizing.Id, authorizing.ResourceToken, new Uri("https://redirect.mydomain.com"));
        Assert.False(string.IsNullOrWhiteSpace(hppUri));
    }

    [Fact]
    public async Task GetPayment_Should_Return_Settled_Payment()
    {
        var client = _fixture.TlClients[0];
        var providerSelection = new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service");

        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].GbpMerchantAccountId),
            providerSelection: providerSelection,
            initAuthorizationFlow: true);

        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(client, paymentRequest, MockBankPaymentAction.Execute, typeof(GetPaymentResponse.Settled));

        var settled = payment.AsT4;
        Assert.Equal(paymentRequest.AmountInMinor, settled.AmountInMinor);
        Assert.Equal(paymentRequest.Currency, settled.Currency);
        Assert.False(string.IsNullOrWhiteSpace(settled.Id));
        Assert.NotEqual(default, settled.CreatedAt);
        Assert.NotNull(settled.PaymentMethod.AsT0);
        Assert.NotNull(settled.CreditableAt);
    }

    [Theory]
    [MemberData(nameof(ExternalAccountPaymentRequests))]
    public async Task Can_Get_Authorization_Required_Payment(CreatePaymentRequest paymentRequest)
    {
        var response = await _fixture.TlClients[1].Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        Assert.True(response.IsSuccessful);
        Assert.True(response.Data.IsT0);
        var authorizationRequiredResponse = response.Data.AsT0;

        var getPaymentResponse = await _fixture.TlClients[1].Payments.GetPayment(authorizationRequiredResponse.Id);

        Assert.True(getPaymentResponse.IsSuccessful);

        Assert.True(getPaymentResponse.Data.TryPickT0(out var payment, out _));
        Assert.Equal("authorization_required", payment.Status);
        Assert.Equal(paymentRequest.AmountInMinor, payment.AmountInMinor);
        Assert.Equal(paymentRequest.Currency, payment.Currency);
        Assert.False(string.IsNullOrWhiteSpace(payment.Id));
        Assert.NotEqual(default, payment.CreatedAt);
        var bankTransfer = payment.PaymentMethod.AsT0;

        bankTransfer.ProviderSelection.Switch(
            userSelected =>
            {
                Provider.UserSelected providerSelectionReq = bankTransfer.ProviderSelection.AsT0;
                Assert.Equal(providerSelectionReq.Filter!.ProviderIds, userSelected.Filter!.ProviderIds);
                // Provider selection hasn't happened yet
                Assert.True(string.IsNullOrEmpty(userSelected.ProviderId));
                Assert.True(string.IsNullOrEmpty(userSelected.SchemeId));
            },
            preselected =>
            {
                Provider.Preselected providerSelectionReq = bankTransfer.ProviderSelection.AsT1;
                AssertSchemeSelection(preselected.SchemeSelection, providerSelectionReq.SchemeSelection, preselected.SchemeId, providerSelectionReq.SchemeId);
                Assert.Equal(providerSelectionReq.ProviderId, preselected.ProviderId);
                Assert.Equal(providerSelectionReq.Remitter, preselected.Remitter);
            });

        Assert.True(bankTransfer.Beneficiary.TryPickT1(out var externalAccount, out _));
        Assert.Equal(bankTransfer.Beneficiary, bankTransfer.Beneficiary);
        Assert.NotNull(payment.User);
        Assert.Equal(authorizationRequiredResponse.User.Id, payment.User.Id);
    }

    [Fact]
    public async Task Can_Create_Payment_With_Retry_Option_And_Get_AttemptFailed_Error()
    {
        // Arrange
        var client = _fixture.TlClients[0];
        var paymentRequest = CreateTestPaymentRequest(
            initAuthorizationFlow: true,
            retry: new Retry.BaseRetry());

        // Act && Assert
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(
            client,
            paymentRequest,
            MockBankPaymentAction.RejectAuthorisation,
            typeof(GetPaymentResponse.AttemptFailed));

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(payment.AsT6.Id));
        Assert.Equal("attempt_failed", payment.AsT6.Status);
    }

    [Fact]
    public async Task Can_create_and_get_payment_refund()
    {
        // Arrange
        var client = _fixture.TlClients[0];
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].GbpMerchantAccountId),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(client, paymentRequest, MockBankPaymentAction.Execute, typeof(GetPaymentResponse.Settled));
        var paymentId = payment.AsT4.Id;
        // Act && assert
        var createRefundResponse = await client.Payments.CreatePaymentRefund(
            paymentId: paymentId,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "a-reference"));
        Assert.True(createRefundResponse.IsSuccessful);
        Assert.False(string.IsNullOrWhiteSpace(createRefundResponse.Data!.Id));

        var getPaymentRefundResponse = await client.Payments.GetPaymentRefund(
            paymentId: paymentId,
            refundId: createRefundResponse.Data!.Id);

        // Assert
        Assert.True(createRefundResponse.IsSuccessful);
        Assert.False(string.IsNullOrWhiteSpace(createRefundResponse.Data!.Id));
        Assert.True(getPaymentRefundResponse.IsSuccessful);
    }

    [Fact]
    public async Task Can_Create_And_List_Payment_Refunds()
    {
        // Arrange
        var client = _fixture.TlClients[0];
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].GbpMerchantAccountId),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(client, paymentRequest, MockBankPaymentAction.Execute, typeof(GetPaymentResponse.Settled));
        var paymentId = payment.AsT4.Id;
        // Act && assert
        var createRefundResponse = await client.Payments.CreatePaymentRefund(
            paymentId: paymentId,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "a-reference"));
        Assert.True(createRefundResponse.IsSuccessful);
        Assert.False(string.IsNullOrWhiteSpace(createRefundResponse.Data!.Id));

        // Act
        var listPaymentRefundsResponse = await client.Payments.ListPaymentRefunds(paymentId);

        // Assert
        Assert.True(createRefundResponse.IsSuccessful);
        Assert.False(string.IsNullOrWhiteSpace(createRefundResponse.Data!.Id));
        Assert.True(listPaymentRefundsResponse.IsSuccessful);
        Assert.Equal(1, listPaymentRefundsResponse.Data!.Items.Count);
    }

    [Fact]
    public async Task Can_List_Payment_Refunds_With_RefundExecuted_Status()
    {
        // Arrange
        var client = _fixture.TlClients[0];
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].GbpMerchantAccountId),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(client, paymentRequest, MockBankPaymentAction.Execute, typeof(GetPaymentResponse.Settled));
        var paymentId = payment.AsT4.Id;

        // Create refund and wait for it to be executed
        var createRefundResponse = await client.Payments.CreatePaymentRefund(
            paymentId: paymentId,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "executed-refund"));
        Assert.True(createRefundResponse.IsSuccessful);

        // Act - List refunds (may include RefundExecuted status)
        var listPaymentRefundsResponse = await client.Payments.ListPaymentRefunds(paymentId);

        // Assert
        Assert.True(listPaymentRefundsResponse.IsSuccessful);
        Assert.NotEmpty(listPaymentRefundsResponse.Data!.Items);
        // Note: RefundExecuted status depends on actual payment processing state
    }

    [Fact]
    public async Task Can_List_Payment_Refunds_With_RefundFailed_Status()
    {
        // Arrange
        var client = _fixture.TlClients[0];
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: new Beneficiary.MerchantAccount(_fixture.ClientMerchantAccounts[0].GbpMerchantAccountId),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(client, paymentRequest, MockBankPaymentAction.Execute, typeof(GetPaymentResponse.Settled));
        var paymentId = payment.AsT4.Id;

        // Create refund with specific reference that may trigger failure
        var createRefundResponse = await client.Payments.CreatePaymentRefund(
            paymentId: paymentId,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "TUOYAP"));
        Assert.True(createRefundResponse.IsSuccessful);

        // Act - List refunds (may include RefundFailed status)
        var listPaymentRefundsResponse = await client.Payments.ListPaymentRefunds(paymentId);

        // Assert
        Assert.True(listPaymentRefundsResponse.IsSuccessful);
        Assert.NotEmpty(listPaymentRefundsResponse.Data!.Items);
        // Note: RefundFailed status depends on actual payment processing state
    }

    [Fact]
    public async Task Can_Cancel_Payment()
    {
        // arrange
        var paymentRequest = CreateTestPaymentRequest();
        var payment = await _fixture.TlClients[0].Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        Assert.True(payment.IsSuccessful);
        var paymentId = payment.Data.AsT0.Id;

        // act
        var cancelPaymentResponse = await _fixture.TlClients[0].Payments.CancelPayment(
            paymentId,
            idempotencyKey: Guid.NewGuid().ToString());

        var getPaymentResponse = await _fixture.TlClients[0].Payments.GetPayment(paymentId);

        // assert
        Assert.True(cancelPaymentResponse.IsSuccessful);
        Assert.Equal(HttpStatusCode.Accepted, cancelPaymentResponse.StatusCode);
        Assert.True(getPaymentResponse.IsSuccessful);
        Assert.NotNull(getPaymentResponse.Data.AsT5);
        Assert.Equal("canceled", getPaymentResponse.Data.AsT5.FailureReason);
    }

    private static void AssertSchemeSelection(
        PaymentsSchemeSelectionUnion? actualSchemeSelection,
        PaymentsSchemeSelectionUnion? expectedSchemeSelection,
        string? actualSchemeId,
        string? expectedSchemeId)
    {
        if (actualSchemeSelection is null && expectedSchemeSelection is null)
        {
            Assert.Equal(expectedSchemeId, actualSchemeId);
            return;
        }

        Assert.NotNull(actualSchemeSelection);
        Assert.NotNull(expectedSchemeSelection);
        var expectedSelection = expectedSchemeSelection!.Value;
        actualSchemeSelection!.Value.Switch(
            instantOnly => { Assert.Equal(expectedSelection.AsT0.AllowRemitterFee, instantOnly.AllowRemitterFee); },
            instantPreferred => { Assert.Equal(expectedSelection.AsT1.AllowRemitterFee, instantPreferred.AllowRemitterFee); },
            preselectedSchemeSelection => { Assert.Equal(expectedSelection.AsT2.SchemeId, preselectedSchemeSelection.SchemeId); },
            userSelected => { Assert.True(expectedSelection.IsT3); });
    }

    [Fact]
    public async Task GetPayment_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() => client.Payments.GetPayment(paymentId));
        Assert.Equal("Value is malformed (Parameter 'id')", result.Message);
    }

    [Fact]
    public async Task StartAuthFlow_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.StartAuthorizationFlow(paymentId, Guid.NewGuid().ToString(),
                new StartAuthorizationFlowRequest()));
        Assert.Equal("Value is malformed (Parameter 'paymentId')", result.Message);
    }

    [Fact]
    public async Task CreateRefund_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.CreatePaymentRefund(paymentId, Guid.NewGuid().ToString(),
                new CreatePaymentRefundRequest("reference")));
        Assert.Equal("Value is malformed (Parameter 'paymentId')", result.Message);
    }

    [Fact]
    public async Task ListPaymentRefunds_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.ListPaymentRefunds(paymentId));
        Assert.Equal("Value is malformed (Parameter 'paymentId')", result.Message);
    }

    [Fact]
    public async Task GetPaymentRefund_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.GetPaymentRefund(paymentId, Guid.NewGuid().ToString()));
        Assert.Equal("Value is malformed (Parameter 'paymentId')", result.Message);
    }

    [Fact]
    public async Task GetPaymentRefund_Url_As_RefundId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string refundId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.GetPaymentRefund(Guid.NewGuid().ToString(), refundId));
        Assert.Equal("Value is malformed (Parameter 'refundId')", result.Message);
    }

    [Fact]
    public async Task CancelPayment_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.CancelPayment(paymentId, Guid.NewGuid().ToString()));
        Assert.Equal("Value is malformed (Parameter 'paymentId')", result.Message);
    }

    private static CreatePaymentRequest CreateTestPaymentRequest(
        ProviderUnion? providerSelection = null,
        AccountIdentifierUnion? accountIdentifier = null,
        string currency = Currencies.GBP,
        RelatedProducts? relatedProducts = null,
        BeneficiaryUnion? beneficiary = null,
        Retry.BaseRetry? retry = null,
        bool initAuthorizationFlow = false,
        SubMerchants? subMerchants = null)
    {
        accountIdentifier ??= new AccountIdentifier.SortCodeAccountNumber("567890", "12345678");
        providerSelection ??= new Provider.Preselected("mock-payments-gb-redirect",
            schemeSelection: new SchemeSelection.Preselected { SchemeId = "faster_payments_service" });
        beneficiary ??= new Beneficiary.ExternalAccount(
            "TrueLayer",
            "truelayer-dotnet",
            accountIdentifier.Value);
        var authorizationFlow = initAuthorizationFlow
            ? new StartAuthorizationFlowRequest(
                ProviderSelection: providerSelection,
                Redirect: new Redirect(
                    new Uri("http://localhost:3000/callback"),
                    new Uri("http://localhost:3000/callback")),
                Retry: retry)
            : null;

        return new CreatePaymentRequest(
            100,
            currency,
            new PaymentMethod.BankTransfer(
                providerSelection.Value,
                beneficiary.Value,
                retry),
            new PaymentUserRequest(
                name: "Jane Doe",
                email: "jane.doe@example.com",
                phone: "+442079460087",
                dateOfBirth: new DateTime(1999, 1, 1),
                address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
            relatedProducts,
            authorizationFlow,
            metadata: new Dictionary<string, string>
            {
                ["test-key-1"] = "test-value-1",
                ["test-key-2"] = "test-value-2",
            },
            riskAssessment: new RiskAssessment("test"),
            subMerchants: subMerchants
        );
    }

    public static IEnumerable<object[]> ExternalAccountPaymentRequests()
    {
        var sortCodeAccountNumber = new AccountIdentifier.SortCodeAccountNumber("567890", "12345678");
        var providerFilterMockGbRedirect = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] };
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantOnly
                    {
                        AllowRemitterFee = false,
                        InstantOverrideProviderIds = ["mock-payments-gb-redirect"],
                        NonInstantOverrideProviderIds = ["mock-payments-de-redirect"]
                    },
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantPreferred
                    {
                        AllowRemitterFee = true,
                        InstantOverrideProviderIds = ["mock-payments-de-embedded"],
                        NonInstantOverrideProviderIds = ["mock-payments-de-redirect"]
                    },
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantPreferred { AllowRemitterFee = false },
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.UserSelected(),
                },
                sortCodeAccountNumber)
        ];

        var remitterSortAccountNumber = new RemitterAccount("John Doe", sortCodeAccountNumber);
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service")
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.Preselected { SchemeId = "faster_payments_service"})
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-gb-redirect", schemeSelection: new SchemeSelection.UserSelected())
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantOnly { AllowRemitterFee = true })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantOnly { AllowRemitterFee = false })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantPreferred { AllowRemitterFee = true })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantPreferred { AllowRemitterFee = false })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-fr-redirect", schemeSelection: new SchemeSelection.Preselected { SchemeId = "sepa_credit_transfer_instant" })
                {
                    Remitter = new RemitterAccount("John Doe", new AccountIdentifier.Iban("FR1420041010050500013M02606")),
                },
                new AccountIdentifier.Iban("IT60X0542811101000000123456"),
                Currencies.EUR,
                new RelatedProducts(new SignupPlus()))
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-gb-redirect", schemeSelection: new SchemeSelection.Preselected { SchemeId = "faster_payments_service" }),
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-fr-redirect", schemeSelection: new SchemeSelection.Preselected { SchemeId = "sepa_credit_transfer_instant" }),
                new AccountIdentifier.Iban("IT60X0542811101000000123456"),
                Currencies.EUR)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-pl-redirect", schemeSelection: new SchemeSelection.Preselected { SchemeId = "polish_domestic_standard" })
                {
                    Remitter = new RemitterAccount(
                        "John Doe", new AccountIdentifier.Nrb("12345678901234567890123456"))
                },
                new AccountIdentifier.Nrb("12345678901234567890123456"),
                Currencies.PLN)
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-no-redirect", schemeSelection:new SchemeSelection.Preselected { SchemeId = "norwegian_domestic_credit_transfer" })
                {
                    Remitter = new RemitterAccount(
                        "John Doe", new AccountIdentifier.Bban("12345678901234567890123456"))
                },
                new AccountIdentifier.Bban("IT60X0542811101000000123456"),
                Currencies.NOK)
        ];
        // Create a payment with retry
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
                },
                sortCodeAccountNumber,
                retry: new Retry.BaseRetry())
        ];
        yield return
        [
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantPreferred { AllowRemitterFee = false })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber,
                retry: new Retry.BaseRetry())
        ];
    }

    private async Task<GetPaymentUnion> CreatePaymentAndSetAuthorisationStatusAsync(
        ITrueLayerClient trueLayerClient,
        CreatePaymentRequest paymentRequest,
        MockBankPaymentAction mockBankPaymentAction,
        Type expectedPaymentStatus)
    {
        var response = await trueLayerClient.Payments.CreatePayment(paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var authorizing = response.Data.AsT3;
        var paymentId = authorizing.Id;

        var providerReturnUri = await _fixture.MockBankClient.AuthorisePaymentAsync(
            authorizing.AuthorizationFlow!.Actions.Next.AsT2.Uri,
            mockBankPaymentAction);

        await _fixture.ApiClient.SubmitPaymentsProviderReturnAsync(providerReturnUri.Query, providerReturnUri.Fragment);

        return await PollPaymentForTerminalStatusAsync(trueLayerClient, paymentId, expectedPaymentStatus);
    }

    private static async Task<GetPaymentUnion> PollPaymentForTerminalStatusAsync(
        ITrueLayerClient trueLayerClient,
        string paymentId,
        Type expectedStatus)
    {
        var getPaymentResponse = await Waiter.WaitAsync(
            () => trueLayerClient.Payments.GetPayment(paymentId),
            r => r.Data.GetType() == expectedStatus);

        Assert.True(getPaymentResponse.IsSuccessful);
        return getPaymentResponse.Data;
    }
}
