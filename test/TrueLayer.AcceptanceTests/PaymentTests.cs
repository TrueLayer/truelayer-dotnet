using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
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

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var authorizationRequired = response.Data.AsT0;

            authorizationRequired.Id.Should().NotBeNullOrWhiteSpace();
            authorizationRequired.ResourceToken.Should().NotBeNullOrWhiteSpace();
            authorizationRequired.User.Should().NotBeNull();
            authorizationRequired.User.Id.Should().NotBeNullOrWhiteSpace();
            authorizationRequired.Status.Should().Be("authorization_required");

            string hppUri = client.Payments.CreateHostedPaymentPageLink(
                authorizationRequired.Id, authorizationRequired.ResourceToken,
                new Uri("https://redirect.mydomain.com"));
            hppUri.Should().NotBeNullOrWhiteSpace();
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var authorizationRequired = response.Data.AsT0;

        authorizationRequired.Id.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.ResourceToken.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.User.Should().NotBeNull();
        authorizationRequired.User.Id.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.Status.Should().Be("authorization_required");

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizationRequired.Id, authorizationRequired.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var authorizationRequired = response.Data.AsT0;

        authorizationRequired.Id.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.ResourceToken.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.User.Should().NotBeNull();
        authorizationRequired.User.Id.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.Status.Should().Be("authorization_required");

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizationRequired.Id, authorizationRequired.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var authorizationRequired = response.Data.AsT0;

        authorizationRequired.Id.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.ResourceToken.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.User.Should().NotBeNull();
        authorizationRequired.User.Id.Should().NotBeNullOrWhiteSpace();
        authorizationRequired.Status.Should().Be("authorization_required");

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizationRequired.Id, authorizationRequired.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Data.IsT3.Should().BeTrue();
        var authorizing = response.Data.AsT3;

        authorizing.Id.Should().NotBeNullOrWhiteSpace();
        authorizing.ResourceToken.Should().NotBeNullOrWhiteSpace();
        authorizing.User.Should().NotBeNull();
        authorizing.User.Id.Should().NotBeNullOrWhiteSpace();
        authorizing.Status.Should().Be("authorizing");
        authorizing.AuthorizationFlow.Should().NotBeNull();
        // The next action is a redirect
        authorizing.AuthorizationFlow!.Actions.Next.IsT2.Should().BeTrue();
        authorizing.AuthorizationFlow.Actions.Next.AsT2.Uri.Should().NotBeNull();

        string hppUri = _fixture.TlClients[0].Payments.CreateHostedPaymentPageLink(
            authorizing.Id, authorizing.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
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
        settled.AmountInMinor.Should().Be(paymentRequest.AmountInMinor);
        settled.Currency.Should().Be(paymentRequest.Currency);
        settled.Id.Should().NotBeNullOrWhiteSpace();
        settled.CreatedAt.Should().NotBe(default);
        settled.PaymentMethod.AsT0.Should().NotBeNull();
        settled.CreditableAt.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(ExternalAccountPaymentRequests))]
    public async Task Can_Get_Authorization_Required_Payment(CreatePaymentRequest paymentRequest)
    {
        var response = await _fixture.TlClients[1].Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.IsSuccessful.Should().BeTrue();
        response.Data.IsT0.Should().BeTrue();
        var authorizationRequiredResponse = response.Data.AsT0;

        var getPaymentResponse = await _fixture.TlClients[1].Payments.GetPayment(authorizationRequiredResponse.Id);

        getPaymentResponse.IsSuccessful.Should().BeTrue();

        getPaymentResponse.Data.TryPickT0(out var payment, out _).Should().BeTrue();
        payment.Status.Should().Be("authorization_required");
        payment.AmountInMinor.Should().Be(paymentRequest.AmountInMinor);
        payment.Currency.Should().Be(paymentRequest.Currency);
        payment.Id.Should().NotBeNullOrWhiteSpace();
        payment.CreatedAt.Should().NotBe(default);
        var bankTransfer = payment.PaymentMethod.AsT0;

        bankTransfer.ProviderSelection.Switch(
            userSelected =>
            {
                Provider.UserSelected providerSelectionReq = bankTransfer.ProviderSelection.AsT0;
                userSelected.Filter.Should().BeEquivalentTo(providerSelectionReq.Filter);
                // Provider selection hasn't happened yet
                userSelected.ProviderId.Should().BeNullOrEmpty();
                userSelected.SchemeId.Should().BeNullOrEmpty();
            },
            preselected =>
            {
                Provider.Preselected providerSelectionReq = bankTransfer.ProviderSelection.AsT1;
                AssertSchemeSelection(preselected.SchemeSelection, providerSelectionReq.SchemeSelection, preselected.SchemeId, providerSelectionReq.SchemeId);
                preselected.ProviderId.Should().Be(providerSelectionReq.ProviderId);
                preselected.Remitter.Should().Be(providerSelectionReq.Remitter);
            });

        bankTransfer.Beneficiary.TryPickT1(out var externalAccount, out _).Should().BeTrue();
        bankTransfer.Beneficiary.Should().Be(bankTransfer.Beneficiary);
        payment.User.Should().NotBeNull();
        payment.User.Id.Should().Be(authorizationRequiredResponse.User.Id);
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
        payment.AsT6.Id.Should().NotBeNullOrWhiteSpace();
        payment.AsT6.Status.Should().Be("attempt_failed");
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
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();

        var getPaymentRefundResponse = await client.Payments.GetPaymentRefund(
            paymentId: paymentId,
            refundId: createRefundResponse.Data!.Id);

        // Assert
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();
        getPaymentRefundResponse.IsSuccessful.Should().BeTrue();
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
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();

        // Act
        var listPaymentRefundsResponse = await client.Payments.ListPaymentRefunds(paymentId);

        // Assert
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();
        listPaymentRefundsResponse.IsSuccessful.Should().BeTrue();
        listPaymentRefundsResponse.Data!.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task Can_Cancel_Payment()
    {
        // arrange
        var paymentRequest = CreateTestPaymentRequest();
        var payment = await _fixture.TlClients[0].Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        payment.IsSuccessful.Should().BeTrue();
        var paymentId = payment.Data.AsT0.Id;

        // act
        var cancelPaymentResponse = await _fixture.TlClients[0].Payments.CancelPayment(
            paymentId,
            idempotencyKey: Guid.NewGuid().ToString());

        var getPaymentResponse = await _fixture.TlClients[0].Payments.GetPayment(paymentId);

        // assert
        cancelPaymentResponse.IsSuccessful.Should().BeTrue();
        cancelPaymentResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        getPaymentResponse.IsSuccessful.Should().BeTrue();
        getPaymentResponse.Data.AsT5.Should().NotBeNull();
        getPaymentResponse.Data.AsT5.FailureReason.Should().Be("canceled");
    }

    private static void AssertSchemeSelection(
        PaymentsSchemeSelectionUnion? actualSchemeSelection,
        PaymentsSchemeSelectionUnion? expectedSchemeSelection,
        string? actualSchemeId,
        string? expectedSchemeId)
    {
        if (actualSchemeSelection is null && expectedSchemeSelection is null)
        {
            actualSchemeId.Should().BeEquivalentTo(expectedSchemeId);
            return;
        }

        actualSchemeSelection.Should().NotBeNull();
        expectedSchemeSelection.Should().NotBeNull();
        var expectedSelection = expectedSchemeSelection!.Value;
        actualSchemeSelection!.Value.Switch(
            instantOnly => { instantOnly.AllowRemitterFee.Should().Be(expectedSelection.AsT0.AllowRemitterFee); },
            instantPreferred => { instantPreferred.AllowRemitterFee.Should().Be(expectedSelection.AsT1.AllowRemitterFee); },
            preselectedSchemeSelection => { preselectedSchemeSelection.SchemeId.Should().Be(expectedSelection.AsT2.SchemeId); },
            userSelected => { expectedSelection.IsT3.Should().Be(true); });
    }

    [Fact]
    public async Task GetPayment_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() => client.Payments.GetPayment(paymentId));
        result.Message.Should().Be("Value is malformed (Parameter 'id')");
    }

    [Fact]
    public async Task StartAuthFlow_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.StartAuthorizationFlow(paymentId, Guid.NewGuid().ToString(),
                new StartAuthorizationFlowRequest()));
        result.Message.Should().Be("Value is malformed (Parameter 'paymentId')");
    }

    [Fact]
    public async Task CreateRefund_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.CreatePaymentRefund(paymentId, Guid.NewGuid().ToString(),
                new CreatePaymentRefundRequest("reference")));
        result.Message.Should().Be("Value is malformed (Parameter 'paymentId')");
    }

    [Fact]
    public async Task ListPaymentRefunds_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.ListPaymentRefunds(paymentId));
        result.Message.Should().Be("Value is malformed (Parameter 'paymentId')");
    }

    [Fact]
    public async Task GetPaymentRefund_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.GetPaymentRefund(paymentId, Guid.NewGuid().ToString()));
        result.Message.Should().Be("Value is malformed (Parameter 'paymentId')");
    }

    [Fact]
    public async Task GetPaymentRefund_Url_As_RefundId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string refundId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.GetPaymentRefund(Guid.NewGuid().ToString(), refundId));
        result.Message.Should().Be("Value is malformed (Parameter 'refundId')");
    }

    [Fact]
    public async Task CancelPayment_Url_As_PaymentId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string paymentId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payments.CancelPayment(paymentId, Guid.NewGuid().ToString()));
        result.Message.Should().Be("Value is malformed (Parameter 'paymentId')");
    }

    private static CreatePaymentRequest CreateTestPaymentRequest(
        ProviderUnion? providerSelection = null,
        AccountIdentifierUnion? accountIdentifier = null,
        string currency = Currencies.GBP,
        RelatedProducts? relatedProducts = null,
        BeneficiaryUnion? beneficiary = null,
        Retry.BaseRetry? retry = null,
        bool initAuthorizationFlow = false)
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
            riskAssessment: new RiskAssessment("test")
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var authorizing = response.Data.AsT3;
        var paymentId = authorizing.Id;

        var providerReturnUri = await _fixture.MockBankClient.AuthorisePaymentAsync(
            authorizing.AuthorizationFlow!.Actions.Next.AsT2.Uri,
            mockBankPaymentAction);

        await _fixture.PayApiClient.SubmitProviderReturnParametersAsync(providerReturnUri.Query, providerReturnUri.Fragment);

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

        getPaymentResponse.IsSuccessful.Should().BeTrue();
        return getPaymentResponse.Data;
    }
}
