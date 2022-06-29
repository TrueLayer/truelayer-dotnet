using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OneOf;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    using ProviderUnion = OneOf<Provider.UserSelected, Provider.Preselected>;
    using AccountIdentifierUnion = OneOf<AccountIdentifier.SortCodeAccountNumber, AccountIdentifier.Iban>;

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
            response.Data.ShouldBeOfType<CreatePaymentResponse>();

            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldNotBeNullOrWhiteSpace();
            response.Data.ResourceToken.ShouldNotBeNullOrWhiteSpace();
            response.Data.User.ShouldNotBeNull();
            response.Data.User.Id.ShouldNotBeNullOrWhiteSpace();

            string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
                response.Data!.Id, response.Data!.ResourceToken, new Uri("https://redirect.mydomain.com"));
            hppUri.ShouldNotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(CreateTestPaymentRequests))]
        public async Task Can_get_authorization_required_payment(CreatePaymentRequest paymentRequest)
        {
            var response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.IsSuccessful.ShouldBeTrue();

            var getPaymentResponse
                = await _fixture.Client.Payments.GetPayment(response.Data!.Id);

            getPaymentResponse.IsSuccessful.ShouldBeTrue();

            getPaymentResponse.Data.TryPickT0(out var payment, out _).ShouldBeTrue();
            payment.Status.ShouldBe("authorization_required");
            payment.AmountInMinor.ShouldBe(paymentRequest.AmountInMinor);
            payment.Currency.ShouldBe(paymentRequest.Currency);
            payment.Id.ShouldNotBeNullOrWhiteSpace();
            payment.CreatedAt.ShouldNotBe(default);
            payment.PaymentMethod.AsT0.ShouldNotBeNull();
            payment.PaymentMethod.AsT0.Beneficiary.TryPickT1(out var externalAccount, out _).ShouldBeTrue();
            payment.User.ShouldNotBeNull();
            payment.User.Id.ShouldBe(response.Data.User.Id);
            payment.User.Name.ShouldBe(paymentRequest.User!.Name);
            payment.User.Email.ShouldBe(paymentRequest.User!.Email);
            payment.User.Phone.ShouldBe(paymentRequest.User!.Phone);
        }

        private static CreatePaymentRequest CreateTestPaymentRequest(
            ProviderUnion providerSelection,
            AccountIdentifierUnion accountIdentifier,
            string currency = Currencies.GBP)
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
                new PaymentUserRequest("Jane Doe", email: "jane.doe@example.com", phone: "+44 1234 567890")
            );

        private static IEnumerable<object[]> CreateTestPaymentRequests()
        {
            var sortCodeAccountNumber = new AccountIdentifier.SortCodeAccountNumber("567890", "12345678");
            yield return new object[]
            {
                CreateTestPaymentRequest(new Provider.UserSelected
                    {
                        Filter = new ProviderFilter {ProviderIds = new[] {"mock-payments-gb-redirect"}},
                    },
                    sortCodeAccountNumber),
            };
            yield return new object[]
            {
                CreateTestPaymentRequest(new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    "faster_payments_service",
                    new RemitterAccount("John Doe", new AccountIdentifier.SortCodeAccountNumber("123456", "12345678"))),
                    sortCodeAccountNumber),
            };
            yield return new object[]
            {
                CreateTestPaymentRequest(new Provider.Preselected(
                    "mock-payments-gb-redirect",
                    "faster_payments_service",
                    new RemitterAccount("John Doe", new AccountIdentifier.Iban("FR1420041010050500013M02606"))),
                    new AccountIdentifier.Iban("IT60X0542811101000000123456"),
                    Currencies.EUR),
            };
        }
    }
}
