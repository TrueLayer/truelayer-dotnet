using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using OneOf;
using TrueLayer.Payments.Model;
using TrueLayer.Payments.Model.AuthorizationFlow;
using Xunit;

namespace TrueLayer.AcceptanceTests;

using SchemeSelectionUnion = OneOf<
    SchemeSelection.UserSelected,
    SchemeSelection.Preselected,
    SchemeSelection.InstantOnly,
    SchemeSelection.InstantPreferred
>;

public partial class PaymentTests
{
    [Theory]
    [MemberData(nameof(CreateTestStartAuthorizationFlowRequests))]
    public async Task Can_start_auth_flow(
        CreatePaymentRequest paymentRequest,
        SchemeSelectionUnion? schemeSelection)
    {
        var paymentResponse = await _fixture.Client.Payments.CreatePayment(
            paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

        PaymentMethod.BankTransfer bankTransfer = paymentRequest.PaymentMethod.AsT0;
        var authFlowRequest = new StartAuthorizationFlowRequest(
            bankTransfer.ProviderSelection,
            schemeSelection,
            new Redirect(new Uri("http://localhost:3000/callback")));

        var response = await _fixture.Client.Payments.StartAuthorizationFlow(
            paymentResponse.Data.AsT0.Id, idempotencyKey: Guid.NewGuid().ToString(), authFlowRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.IsT0.Should().BeTrue();
        AuthorizationFlowResponse.AuthorizationFlowAuthorizing authorizing = response.Data.AsT0;
        authorizing.Status.Should().Be("authorizing");
        authorizing.AuthorizationFlow.Should().NotBeNull();
        authorizing.AuthorizationFlow.Actions.Next.Value.Should().NotBeNull();
    }

    public static IEnumerable<object?[]> CreateTestStartAuthorizationFlowRequests()
    {
        var sortCodeAccountNumber = new AccountIdentifier.SortCodeAccountNumber("567890", "12345678");
        var providerFilterMockGbRedirect = new ProviderFilter { ProviderIds = ["mock-payments-gb-redirect"] };
        var instantOnlyWithRemitterFee = new SchemeSelection.InstantOnly { AllowRemitterFee = false };
        var instantOnlyWithoutRemitterFee = new SchemeSelection.InstantOnly { AllowRemitterFee = false };
        var instantPreferredWithRemitterFee = new SchemeSelection.InstantPreferred() { AllowRemitterFee = true };
        var instantPreferredWithoutRemitterFee = new SchemeSelection.InstantPreferred() { AllowRemitterFee = false };
        var userSelected = new SchemeSelection.UserSelected();
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                    {
                        Filter = providerFilterMockGbRedirect,
                        SchemeSelection = instantOnlyWithRemitterFee,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantOnlyWithRemitterFee

        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                    {
                        Filter = providerFilterMockGbRedirect,
                        SchemeSelection = instantOnlyWithoutRemitterFee,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantOnlyWithoutRemitterFee
        ];

        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                    {
                        Filter = providerFilterMockGbRedirect,
                        SchemeSelection = instantPreferredWithRemitterFee,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantOnlyWithRemitterFee
        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                    {
                        Filter = providerFilterMockGbRedirect,
                        SchemeSelection = instantPreferredWithoutRemitterFee,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantOnlyWithoutRemitterFee
        ];
        yield return
        [
            CreateTestPaymentRequest(new Provider.UserSelected
                    {
                        Filter = providerFilterMockGbRedirect,
                        SchemeSelection = userSelected,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)userSelected
        ];

        var remitterSortAccountNumber = new RemitterAccount("John Doe", sortCodeAccountNumber);
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service")
                    {
                        Remitter = remitterSortAccountNumber,
                    },
                    sortCodeAccountNumber),
                null
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected(
                        "mock-payments-gb-redirect",
                        schemeSelection: new SchemeSelection.Preselected() { SchemeId = "faster_payments_service"})
                    {
                        Remitter = remitterSortAccountNumber,
                    },
                    sortCodeAccountNumber),
                null
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected("mock-payments-gb-redirect", schemeSelection: userSelected)
                    {
                        Remitter = remitterSortAccountNumber,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)userSelected
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected(
                        "mock-payments-gb-redirect",
                        schemeSelection: instantOnlyWithRemitterFee)
                    {
                        Remitter = remitterSortAccountNumber,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantOnlyWithRemitterFee
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected(
                        "mock-payments-gb-redirect",
                        schemeSelection: instantOnlyWithoutRemitterFee)
                    {
                        Remitter = remitterSortAccountNumber,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantOnlyWithoutRemitterFee
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected(
                        "mock-payments-gb-redirect",
                        schemeSelection: instantPreferredWithRemitterFee)
                    {
                        Remitter = remitterSortAccountNumber,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantPreferredWithRemitterFee
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected(
                        "mock-payments-gb-redirect",
                        schemeSelection: instantPreferredWithRemitterFee)
                    {
                        Remitter = remitterSortAccountNumber,
                    },
                    sortCodeAccountNumber),
                (SchemeSelectionUnion?)instantPreferredWithRemitterFee
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected("mock-payments-fr-redirect", "sepa_credit_transfer_instant")
                    {
                        Remitter = new RemitterAccount("John Doe", new AccountIdentifier.Iban("FR1420041010050500013M02606")),
                    },
                    new AccountIdentifier.Iban("IT60X0542811101000000123456"),
                    Currencies.EUR,
                    new RelatedProducts(new SignupPlus())),
                null
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service"),
                    sortCodeAccountNumber),
                null
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected("mock-payments-fr-redirect", "sepa_credit_transfer_instant"),
                    new AccountIdentifier.Iban("IT60X0542811101000000123456"),
                    Currencies.EUR),
                null
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected("mock-payments-pl-redirect", "polish_domestic_standard")
                    {
                        Remitter = new RemitterAccount(
                            "John Doe", new AccountIdentifier.Nrb("12345678901234567890123456")),
                    },
                    new AccountIdentifier.Nrb("12345678901234567890123456"),
                    Currencies.PLN),
                null
        ];
        yield return
        [
            CreateTestPaymentRequest(
                    new Provider.Preselected("mock-payments-no-redirect", "norwegian_domestic_credit_transfer")
                    {
                        Remitter = new RemitterAccount(
                            "John Doe", new AccountIdentifier.Bban("12345678901234567890123456")),
                    },
                    new AccountIdentifier.Bban("IT60X0542811101000000123456"),
                    Currencies.NOK),
                null
        ];
    }
}
