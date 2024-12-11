using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneOf;
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
    private readonly TrueLayerOptions _configuration;
    private readonly string _gbpMerchantAccountId;
    private readonly string _eurMerchantSecretKey;

    public PaymentTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
        _configuration = fixture.ServiceProvider.GetRequiredService<IOptions<TrueLayerOptions>>().Value;
        (string gbpMerchantAccountId, string eurMerchantAccountId) = GetMerchantBeneficiaryAccountsAsync().Result;
        _gbpMerchantAccountId = gbpMerchantAccountId;
        _eurMerchantSecretKey = eurMerchantAccountId;
    }

    [Theory]
    [MemberData(nameof(ExternalAccountPaymentRequests))]
    public async Task Can_Create_External_Account_Payment(CreatePaymentRequest paymentRequest)
    {
        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Data.IsT0.Should().BeTrue();
        response.Data.AsT0.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.ResourceToken.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.User.Should().NotBeNull();
        response.Data.AsT0.User.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.Status.Should().Be("authorization_required");

        string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
            response.Data.AsT0.Id, response.Data.AsT0.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task can_create_merchant_account_gbp_Payment()
    {
        var paymentRequest = CreateTestPaymentRequest(
            new Provider.UserSelected
            {
                Filter = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] },
                SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
            },
            beneficiary: new Beneficiary.MerchantAccount(_gbpMerchantAccountId)
            {
                AccountHolderName = "account holder name",
                StatementReference = "statement-ref"
            });

        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Data.IsT0.Should().BeTrue();
        response.Data.AsT0.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.ResourceToken.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.User.Should().NotBeNull();
        response.Data.AsT0.User.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.Status.Should().Be("authorization_required");

        string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
            response.Data.AsT0.Id, response.Data.AsT0.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
    }


    [Fact]
    public async Task can_create_merchant_account_gbp_verification_Payment()
    {
        var paymentRequest = CreateTestPaymentRequest(
            new Provider.UserSelected
            {
                Filter = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] },
                SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = true },
            },
            beneficiary: new Beneficiary.MerchantAccount(_gbpMerchantAccountId)
            {
                Verification = new Verification.Automated { RemitterName = true }
            });

        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Data.IsT0.Should().BeTrue();
        response.Data.AsT0.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.ResourceToken.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.User.Should().NotBeNull();
        response.Data.AsT0.User.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.Status.Should().Be("authorization_required");

        string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
            response.Data.AsT0.Id, response.Data.AsT0.ResourceToken, new Uri("https://redirect.mydomain.com"));
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
            new Beneficiary.MerchantAccount(_eurMerchantSecretKey));

        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Data.IsT0.Should().BeTrue();
        response.Data.AsT0.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.ResourceToken.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.User.Should().NotBeNull();
        response.Data.AsT0.User.Id.Should().NotBeNullOrWhiteSpace();
        response.Data.AsT0.Status.Should().Be("authorization_required");

        string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
            response.Data.AsT0.Id, response.Data.AsT0.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Can_Create_payment_with_auth_flow()
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
        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Data.IsT3.Should().BeTrue();
        CreatePaymentResponse.Authorizing authorizing = response.Data.AsT3;
        authorizing.Id.Should().NotBeNullOrWhiteSpace();
        authorizing.ResourceToken.Should().NotBeNullOrWhiteSpace();
        authorizing.User.Should().NotBeNull();
        authorizing.User.Id.Should().NotBeNullOrWhiteSpace();
        authorizing.Status.Should().Be("authorizing");
        authorizing.AuthorizationFlow.Should().NotBeNull();
        // The next action is a redirect
        authorizing.AuthorizationFlow!.Actions.Next.IsT2.Should().BeTrue();
        authorizing.AuthorizationFlow.Actions.Next.AsT2.Uri.Should().NotBeNull();

        string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
            authorizing.Id, authorizing.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(ExternalAccountPaymentRequests))]
    public async Task Can_Get_Authorization_Required_Payment(CreatePaymentRequest paymentRequest)
    {
        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.IsSuccessful.Should().BeTrue();
        response.Data.IsT0.Should().BeTrue();
        var authorizationRequiredResponse = response.Data.AsT0;

        var getPaymentResponse
            = await _fixture.Client.Payments.GetPayment(authorizationRequiredResponse.Id);

        getPaymentResponse.IsSuccessful.Should().BeTrue();

        getPaymentResponse.Data.TryPickT0(out var payment, out _).Should().BeTrue();
        payment.Status.Should().Be("authorization_required");
        payment.AmountInMinor.Should().Be(paymentRequest.AmountInMinor);
        payment.Currency.Should().Be(paymentRequest.Currency);
        payment.Id.Should().NotBeNullOrWhiteSpace();
        payment.CreatedAt.Should().NotBe(default);
        payment.PaymentMethod.AsT0.Should().NotBeNull();

        payment.PaymentMethod.AsT0.ProviderSelection.Switch(
            userSelected =>
            {
                Provider.UserSelected providerSelectionReq = paymentRequest.PaymentMethod.AsT0.ProviderSelection.AsT0;
                userSelected.Filter.Should().BeEquivalentTo(providerSelectionReq.Filter);
                // Provider selection hasn't happened yet
                userSelected.ProviderId.Should().BeNullOrEmpty();
                userSelected.SchemeId.Should().BeNullOrEmpty();
            },
            preselected =>
            {
                Provider.Preselected providerSelectionReq = paymentRequest.PaymentMethod.AsT0.ProviderSelection.AsT1;
                AssertSchemeSelection(preselected.SchemeSelection, providerSelectionReq.SchemeSelection, preselected.SchemeId, providerSelectionReq.SchemeId);
                preselected.ProviderId.Should().Be(providerSelectionReq.ProviderId);
                preselected.Remitter.Should().Be(providerSelectionReq.Remitter);
            });

        payment.PaymentMethod.AsT0.Beneficiary.TryPickT1(out var externalAccount, out _).Should().BeTrue();
        payment.PaymentMethod.AsT0.Beneficiary.Should().Be(paymentRequest.PaymentMethod.AsT0.Beneficiary);
        payment.User.Should().NotBeNull();
        payment.User.Id.Should().Be(authorizationRequiredResponse.User.Id);
    }

    [Fact]
    public async Task Can_Create_Payment_With_Retry_Option_And_Get_AttemptFailed_Error()
    {
        // Arrange
        var paymentRequest = CreateTestPaymentRequest(
            initAuthorizationFlow: true,
            retry: new Retry.BaseRetry());

        // Act && Assert
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(
            paymentRequest,
            HeadlessResourceAction.RejectAuthorisation,
            typeof(GetPaymentResponse.AttemptFailed));

        // Assert
        payment.Id.Should().NotBeNullOrWhiteSpace();
        payment.Status.Should().Be("attempt_failed");
    }

    [Fact]
    public async Task Can_create_and_get_payment_refund()
    {
        // Arrange
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: new Beneficiary.MerchantAccount(_gbpMerchantAccountId),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(paymentRequest);

        // Act && assert
        var createRefundResponse = await _fixture.Client.Payments.CreatePaymentRefund(
            paymentId: payment.Id,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "a-reference"));
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();

        var getPaymentRefundResponse = await _fixture.Client.Payments.GetPaymentRefund(
            paymentId: payment.Id,
            refundId: createRefundResponse.Data!.Id);

        // Assert
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();
        getPaymentRefundResponse.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task Can_create_and_list_payment_refunds()
    {
        // Arrange
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: new Beneficiary.MerchantAccount(_gbpMerchantAccountId),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(paymentRequest);

        // Act && assert
        var createRefundResponse = await _fixture.Client.Payments.CreatePaymentRefund(
            paymentId: payment.Id,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "a-reference"));
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();

        // Act
        var listPaymentRefundsResponse = await _fixture.Client.Payments.ListPaymentRefunds(payment.Id);

        // Assert
        createRefundResponse.IsSuccessful.Should().BeTrue();
        createRefundResponse.Data!.Id.Should().NotBeNullOrWhiteSpace();
        listPaymentRefundsResponse.IsSuccessful.Should().BeTrue();
        listPaymentRefundsResponse.Data!.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task Can_cancel_a_payment()
    {
        // arrange
        var paymentRequest = CreateTestPaymentRequest();
        var payment = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        payment.IsSuccessful.Should().BeTrue();
        var paymentId = payment.Data.AsT0.Id;

        // act
        var cancelPaymentResponse = await _fixture.Client.Payments.CancelPayment(
            paymentId,
            idempotencyKey: Guid.NewGuid().ToString());

        var getPaymentResponse = await _fixture.Client.Payments.GetPayment(paymentId);

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

    private async Task<GetPaymentResponse.PaymentDetails> CreatePaymentAndSetAuthorisationStatusAsync(
        CreatePaymentRequest paymentRequest,
        HeadlessResourceAction action = HeadlessResourceAction.Execute,
        Type? expectedPaymentStatusTypeOrSettled = null)
    {
        var createdPaymentResponse = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        createdPaymentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        (Uri redirectUri, string paymentId) = createdPaymentResponse.Data.Value switch
        {
            CreatePaymentResponse.Authorizing authorizing => HandleAuthorizingPayment(authorizing),
            _ => throw new InvalidOperationException("Unexpected payment status")
        };

        redirectUri.Should().NotBeNull();

        await TestUtils.RunAndAssertHeadlessResourceAuthorisation(
            _configuration,
            redirectUri,
            HeadlessResourceAuthorization.New(HeadlessResource.Payments, action));

        expectedPaymentStatusTypeOrSettled ??= typeof(GetPaymentResponse.Settled);
        return GetPaymentDetailsAndWaitForItToBeDone(
            paymentId,
            expectedPaymentStatusTypeOrSettled);
    }

    private static (Uri RedirectUri, string PaymentId) HandleAuthorizingPayment(CreatePaymentResponse.Authorizing? authorizing)
    {
        authorizing.Should().NotBeNull();
        authorizing!.Status.Should().Be("authorizing");
        authorizing.AuthorizationFlow.Should().NotBeNull();
        // The next action is a redirect
        return authorizing.AuthorizationFlow!.Actions.Next.Value switch
        {
            Models.AuthorizationFlowAction.Redirect redirect => (redirect.Uri, authorizing.Id),
            _ => throw new InvalidOperationException("Unexpected next action")
        };
    }

    private async Task<(string gbpMerchantAccountId, string eurMerchantAccountId)> GetMerchantBeneficiaryAccountsAsync()
    {
        var merchantAccounts = await _fixture.Client.MerchantAccounts.ListMerchantAccounts();
        merchantAccounts.IsSuccessful.Should().BeTrue();

        var gbpMerchantAccount = merchantAccounts.Data!.Items.First(m => m.Currency == Currencies.GBP);
        var eurMerchantAccount = merchantAccounts.Data!.Items.First(m => m.Currency == Currencies.EUR);
        return (gbpMerchantAccount.Id, eurMerchantAccount.Id);
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
                    SchemeSelection = new SchemeSelection.InstantOnly { AllowRemitterFee = false },
                },
                sortCodeAccountNumber)
        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantPreferred { AllowRemitterFee = true },
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

    private GetPaymentResponse.PaymentDetails GetPaymentDetailsAndWaitForItToBeDone(
        string paymentId,
        Type expectedPaymentStatusType,
        TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        if (Debugger.IsAttached)
        {
            timeout = TimeSpan.FromMinutes(10);
        }

        GetPaymentUnion payment = default;
        var completedGracefully = Task.Run(async () =>
        {
            bool isDone;
            do
            {
                var response = await _fixture.Client.Payments.GetPayment(paymentId);

                // Exit immediately if there's an error
                response.IsSuccessful.Should().BeTrue();
                payment = response.Data;

                isDone = payment.Value.GetType() == expectedPaymentStatusType;

                await Task.Delay(TimeSpan.FromSeconds(1));
            } while (!isDone);
        }).Wait(timeout.Value);

        if (!completedGracefully)
        {
            throw new Exception("Payment did not complete in time");
        }

        return (payment.Value as GetPaymentResponse.PaymentDetails)!;
    }
}
