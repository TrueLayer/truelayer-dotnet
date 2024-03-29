using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OneOf;
using Shouldly;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    using ProviderUnion = OneOf<Provider.UserSelected, Provider.Preselected>;
    using AccountIdentifierUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Iban,
        AccountIdentifier.Bban,
        AccountIdentifier.Nrb>;
    using PreselectedProviderSchemeSelectionUnion = OneOf<SchemeSelection.InstantOnly, SchemeSelection.InstantPreferred, SchemeSelection.Preselected, SchemeSelection.UserSelected>;
    using UserSelectedProviderSchemeSelectionUnion = OneOf<SchemeSelection.InstantOnly, SchemeSelection.InstantPreferred, SchemeSelection.UserSelected>;

    public class PaymentTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public PaymentTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
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

        private static void AssertSchemeSelection(
            PreselectedProviderSchemeSelectionUnion? actualSchemeSelection,
            PreselectedProviderSchemeSelectionUnion? expectedSchemeSelection,
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

        private static CreatePaymentRequest CreateTestPaymentRequest(
            ProviderUnion providerSelection,
            AccountIdentifierUnion accountIdentifier,
            string currency = Currencies.GBP,
            RelatedProducts? relatedProducts = null)
            => new CreatePaymentRequest(
                100,
                currency,
                new PaymentMethod.BankTransfer(
                    providerSelection,
                    new Beneficiary.ExternalAccount(
                        "TrueLayer",
                        "truelayer-dotnet",
                        accountIdentifier
                    )),
                new PaymentUserRequest(
                    name: "Jane Doe",
                    email: "jane.doe@example.com",
                    phone: "+442079460087",
                    dateOfBirth: new DateTime(1999, 1, 1),
                    address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
                relatedProducts
            );

        private static IEnumerable<object[]> CreateTestPaymentRequests()
        {
            var sortCodeAccountNumber = new AccountIdentifier.SortCodeAccountNumber("567890", "12345678");
            var providerFilterMockGbRedirect = new ProviderFilter {ProviderIds = new[] {"mock-payments-gb-redirect"}};
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
        }
    }
}
