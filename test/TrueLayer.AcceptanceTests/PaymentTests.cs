using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneOf;
using Shouldly;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using TrueLayer.Payments.Model.AuthorizationFlow;
using Xunit;
using Xunit.Abstractions;

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
    private readonly ITestOutputHelper _testOutputHelper;

    public PaymentTests(ApiTestFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
        _configuration = fixture.ServiceProvider.GetRequiredService<IOptions<TrueLayerOptions>>().Value;
    }

    [Theory]
    [MemberData(nameof(CreateTestPaymentRequests))]
    public async Task Can_create_payment(CreatePaymentRequest paymentRequest)
    {
        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Data.IsT0.ShouldBeTrue();
        response.Data.AsT0.Id.ShouldNotBeNullOrWhiteSpace();
        response.Data.AsT0.ResourceToken.ShouldNotBeNullOrWhiteSpace();
        response.Data.AsT0.User.ShouldNotBeNull();
        response.Data.AsT0.User.Id.ShouldNotBeNullOrWhiteSpace();
        response.Data.AsT0.Status.ShouldBe("authorization_required");

        string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
            response.Data.AsT0.Id, response.Data.AsT0.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Can_create_payment_with_auth_flow()
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

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Data.IsT3.ShouldBeTrue();
        CreatePaymentResponse.Authorizing authorizing = response.Data.AsT3;
        authorizing.Id.ShouldNotBeNullOrWhiteSpace();
        authorizing.ResourceToken.ShouldNotBeNullOrWhiteSpace();
        authorizing.User.ShouldNotBeNull();
        authorizing.User.Id.ShouldNotBeNullOrWhiteSpace();
        authorizing.Status.ShouldBe("authorizing");
        authorizing.AuthorizationFlow.ShouldNotBeNull();
        // The next action is a redirect
        authorizing.AuthorizationFlow.Actions.Next.IsT2.ShouldBeTrue();
        authorizing.AuthorizationFlow.Actions.Next.AsT2.Uri.ShouldNotBeNull();

        string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
            authorizing.Id, authorizing.ResourceToken, new Uri("https://redirect.mydomain.com"));
        hppUri.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(CreateTestPaymentRequests))]
    public async Task Can_get_authorization_required_payment(CreatePaymentRequest paymentRequest)
    {
        var response = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.IsSuccessful.ShouldBeTrue();
        response.Data.IsT0.ShouldBeTrue();
        var authorizationRequiredResponse = response.Data.AsT0;

        var getPaymentResponse
            = await _fixture.Client.Payments.GetPayment(authorizationRequiredResponse.Id);

        getPaymentResponse.IsSuccessful.ShouldBeTrue();

        getPaymentResponse.Data.TryPickT0(out var payment, out _).ShouldBeTrue();
        payment.Status.ShouldBe("authorization_required");
        payment.AmountInMinor.ShouldBe(paymentRequest.AmountInMinor);
        payment.Currency.ShouldBe(paymentRequest.Currency);
        payment.Id.ShouldNotBeNullOrWhiteSpace();
        payment.CreatedAt.ShouldNotBe(default);
        payment.PaymentMethod.AsT0.ShouldNotBeNull();

        payment.PaymentMethod.AsT0.ProviderSelection.Switch(
            userSelected =>
            {
                Provider.UserSelected providerSelectionReq = paymentRequest.PaymentMethod.AsT0.ProviderSelection.AsT0;
                userSelected.Filter.ShouldBeEquivalentTo(providerSelectionReq.Filter);
                // Provider selection hasn't happened yet
                userSelected.ProviderId.ShouldBeNullOrEmpty();
                userSelected.SchemeId.ShouldBeNullOrEmpty();
            },
            preselected =>
            {
                Provider.Preselected providerSelectionReq = paymentRequest.PaymentMethod.AsT0.ProviderSelection.AsT1;
                AssertSchemeSelection(preselected.SchemeSelection, providerSelectionReq.SchemeSelection, preselected.SchemeId, providerSelectionReq.SchemeId);
                preselected.ProviderId.ShouldBe(providerSelectionReq.ProviderId);
                preselected.Remitter.ShouldBe(providerSelectionReq.Remitter);
            });

        payment.PaymentMethod.AsT0.Beneficiary.TryPickT1(out var externalAccount, out _).ShouldBeTrue();
        payment.PaymentMethod.AsT0.Beneficiary.ShouldBe(paymentRequest.PaymentMethod.AsT0.Beneficiary);
        payment.User.ShouldNotBeNull();
        payment.User.Id.ShouldBe(authorizationRequiredResponse.User.Id);
    }

    [Fact]
    public async Task Can_create_payment_with_retry_option_and_get_attemptFailed_error()
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
        payment.Id.ShouldNotBeNullOrWhiteSpace();
        payment.Status.ShouldBe("attempt_failed");
    }

    [Fact]
    public async Task Can_create_and_get_payment_refund()
    {
        // Arrange
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: await CreateMerchantBeneficiaryAsync(),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(paymentRequest);

        // Act && assert
        var createRefundResponse = await _fixture.Client.Payments.CreatePaymentRefund(
            paymentId: payment.Id,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "a-reference"));
        createRefundResponse.IsSuccessful.ShouldBeTrue();
        createRefundResponse.Data!.Id.ShouldNotBeNullOrWhiteSpace();

        var getPaymentRefundResponse = await _fixture.Client.Payments.GetPaymentRefund(
            paymentId: payment.Id,
            refundId: createRefundResponse.Data!.Id);

        // Assert
        createRefundResponse.IsSuccessful.ShouldBeTrue();
        createRefundResponse.Data!.Id.ShouldNotBeNullOrWhiteSpace();
        getPaymentRefundResponse.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public async Task Can_create_and_list_payment_refunds()
    {
        // Arrange
        var paymentRequest = CreateTestPaymentRequest(
            beneficiary: await CreateMerchantBeneficiaryAsync(),
            initAuthorizationFlow: true);
        var payment = await CreatePaymentAndSetAuthorisationStatusAsync(paymentRequest);

        // Act && assert
        var createRefundResponse = await _fixture.Client.Payments.CreatePaymentRefund(
            paymentId: payment.Id,
            idempotencyKey: Guid.NewGuid().ToString(),
            new CreatePaymentRefundRequest(Reference: "a-reference"));
        createRefundResponse.IsSuccessful.ShouldBeTrue();
        createRefundResponse.Data!.Id.ShouldNotBeNullOrWhiteSpace();

        // Act
        var listPaymentRefundsResponse = await _fixture.Client.Payments.ListPaymentRefunds(payment.Id);

        // Assert
        createRefundResponse.IsSuccessful.ShouldBeTrue();
        createRefundResponse.Data!.Id.ShouldNotBeNullOrWhiteSpace();
        listPaymentRefundsResponse.IsSuccessful.ShouldBeTrue();
        listPaymentRefundsResponse.Data!.Items.Count.ShouldBe(1);
    }

    private static void AssertSchemeSelection(
        PaymentsSchemeSelectionUnion? actualSchemeSelection,
        PaymentsSchemeSelectionUnion? expectedSchemeSelection,
        string? actualSchemeId,
        string? expectedSchemeId)
    {
        if (actualSchemeSelection is null && expectedSchemeSelection is null)
        {
            actualSchemeId.ShouldBeEquivalentTo(expectedSchemeId);
            return;
        }

        actualSchemeSelection.ShouldNotBeNull();
        expectedSchemeSelection.ShouldNotBeNull();
        var expectedSelection = expectedSchemeSelection.Value;
        actualSchemeSelection.Value.Switch(
            instantOnly => { instantOnly.AllowRemitterFee.ShouldBe(expectedSelection.AsT0.AllowRemitterFee); },
            instantPreferred => { instantPreferred.AllowRemitterFee.ShouldBe(expectedSelection.AsT1.AllowRemitterFee); },
            preselectedSchemeSelection => { preselectedSchemeSelection.SchemeId.ShouldBe(expectedSelection.AsT2.SchemeId); },
            userSelected => { expectedSelection.IsT3.ShouldBe(true); });
    }

    private async Task<GetPaymentResponse.PaymentDetails> CreatePaymentAndSetAuthorisationStatusAsync(
        CreatePaymentRequest paymentRequest,
        HeadlessResourceAction action = HeadlessResourceAction.Execute,
        Type? expectedPaymentStatusTypeOrSettled = null)
    {
        var createdPaymentResponse = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        createdPaymentResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        (Uri redirectUri, string paymentId) = createdPaymentResponse.Data.Value switch
        {
            CreatePaymentResponse.Authorizing authorizing => HandleAuthorizingPayment(authorizing),
            _ => throw new InvalidOperationException("Unexpected payment status")
        };

        redirectUri.ShouldNotBeNull();

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
        authorizing.ShouldNotBeNull();
        authorizing.Status.ShouldBe("authorizing");
        authorizing.AuthorizationFlow.ShouldNotBeNull();
        // The next action is a redirect
        return authorizing.AuthorizationFlow.Actions.Next.Value switch
        {
            Models.AuthorizationFlowAction.Redirect redirect => (redirect.Uri, authorizing.Id),
            _ => throw new InvalidOperationException("Unexpected next action")
        };
    }

    private async Task<BeneficiaryUnion> CreateMerchantBeneficiaryAsync()
    {
        var merchantAccounts = await _fixture.Client.MerchantAccounts.ListMerchantAccounts();
            merchantAccounts.IsSuccessful.ShouldBeTrue();
            var gbpMerchantAccount = merchantAccounts.Data!.Items.First(m => m.Currency == Currencies.GBP);
        return new Beneficiary.MerchantAccount(gbpMerchantAccount.Id);
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
        providerSelection ??= new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service");
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
            authorizationFlow
        );
    }

    private static IEnumerable<object[]> CreateTestPaymentRequests()
    {
        var sortCodeAccountNumber = new AccountIdentifier.SortCodeAccountNumber("567890", "12345678");
        var providerFilterMockGbRedirect = new ProviderFilter { ProviderIds = new[] { "mock-payments-gb-redirect" } };
        yield return new object[]
        {
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantOnly() { AllowRemitterFee = true },
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantOnly() { AllowRemitterFee = false },
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantPreferred() { AllowRemitterFee = true },
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantPreferred() { AllowRemitterFee = false },
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.UserSelected(),
                },
                sortCodeAccountNumber),
        };

        var remitterSortAccountNumber = new RemitterAccount("John Doe", sortCodeAccountNumber);
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service")
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.Preselected() { SchemeId = "faster_payments_service"})
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-gb-redirect", schemeSelection: new SchemeSelection.UserSelected())
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantOnly() { AllowRemitterFee = true })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantOnly() { AllowRemitterFee = false })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantPreferred() { AllowRemitterFee = true })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantPreferred() { AllowRemitterFee = false })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-fr-redirect", "sepa_credit_transfer_instant")
                {
                    Remitter = new RemitterAccount("John Doe", new AccountIdentifier.Iban("FR1420041010050500013M02606")),
                },
                new AccountIdentifier.Iban("IT60X0542811101000000123456"),
                Currencies.EUR,
                new RelatedProducts(new SignupPlus())),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service"),
                sortCodeAccountNumber),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-fr-redirect", "sepa_credit_transfer_instant"),
                new AccountIdentifier.Iban("IT60X0542811101000000123456"),
                Currencies.EUR),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-pl-redirect", "polish_domestic_standard")
                {
                    Remitter = new RemitterAccount(
                        "John Doe", new AccountIdentifier.Nrb("12345678901234567890123456")),
                },
                new AccountIdentifier.Nrb("12345678901234567890123456"),
                Currencies.PLN),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected("mock-payments-no-redirect", "norwegian_domestic_credit_transfer")
                {
                    Remitter = new RemitterAccount(
                        "John Doe", new AccountIdentifier.Bban("12345678901234567890123456")),
                },
                new AccountIdentifier.Bban("IT60X0542811101000000123456"),
                Currencies.NOK),
        };
        // Create a payment with retry
        yield return new object[]
        {
            CreateTestPaymentRequest(new Provider.UserSelected
                {
                    Filter = providerFilterMockGbRedirect,
                    SchemeSelection = new SchemeSelection.InstantOnly() { AllowRemitterFee = true },
                },
                sortCodeAccountNumber,
                retry: new Retry.BaseRetry()),
        };
        yield return new object[]
        {
            CreateTestPaymentRequest(
                new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    schemeSelection: new SchemeSelection.InstantPreferred() { AllowRemitterFee = false })
                {
                    Remitter = remitterSortAccountNumber,
                },
                sortCodeAccountNumber,
                retry: new Retry.BaseRetry()),
        };
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
                response.IsSuccessful.ShouldBeTrue();
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
