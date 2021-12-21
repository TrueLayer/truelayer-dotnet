using System;
using System.Net;
using System.Threading.Tasks;
using OneOf;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;
using static TrueLayer.Payments.Model.PaymentUser;

namespace TrueLayer.AcceptanceTests
{
    public class PaymentTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public PaymentTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_create_payment()
        {
            CreatePaymentRequest paymentRequest = CreatePaymentRequest();

            var response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Data.Value.ShouldBeOfType<CreatePaymentResponse.AuthorizationRequired>();

            string hppUri = response.Data.Match(
                authRequired =>
                {
                    authRequired.Status.ShouldBe("authorization_required");

                    authRequired.AmountInMinor.ShouldBe(paymentRequest.AmountInMinor);
                    authRequired.Currency.ShouldBe(paymentRequest.Currency);
                    authRequired.Id.ShouldNotBeNullOrWhiteSpace();
                    authRequired.ResourceToken.ShouldNotBeNullOrWhiteSpace();
                    authRequired.CreatedAt.ShouldNotBe(default);
                    authRequired.User.ShouldNotBeNull();
                    authRequired.User.Id.ShouldNotBeNullOrWhiteSpace();
                    authRequired.User.Name.ShouldBe(paymentRequest.User.AsT0.Name);
                    authRequired.User.Email.ShouldBe(paymentRequest.User.AsT0.Email);
                    authRequired.User.Phone.ShouldBe(paymentRequest.User.AsT0.Phone);

                    return _fixture.Client.Payments.CreateHostedPaymentPageLink(
                        authRequired.Id, authRequired.ResourceToken, new Uri("https://redirect.mydomain.com")
                    );
                }
            );

            hppUri.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Can_get_authorization_required_payment()
        {
            CreatePaymentRequest paymentRequest = CreatePaymentRequest();

            var response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

            response.IsSuccessful.ShouldBeTrue();

            var getPaymentResponse
                = await _fixture.Client.Payments.GetPayment(response.Data.AsT0.Id);

            getPaymentResponse.IsSuccessful.ShouldBeTrue();

            getPaymentResponse.Data.TryPickT0(out var payment, out _).ShouldBeTrue();
            payment.Status.ShouldBe("authorization_required");
            payment.AmountInMinor.ShouldBe(paymentRequest.AmountInMinor);
            payment.Currency.ShouldBe(paymentRequest.Currency);
            payment.Id.ShouldNotBeNullOrWhiteSpace();
            payment.CreatedAt.ShouldNotBe(default);
            payment.Beneficiary.TryPickT1(out var externalAccount, out _).ShouldBeTrue();
            payment.PaymentMethod.AsT0.ShouldNotBeNull();
            payment.User.ShouldNotBeNull();
            payment.User.Id.ShouldBe(response.Data.AsT0.User.Id);

            // TODO Currently hardcoded in v3. Remove once implementation is complete
            //payment.User.Name.ShouldBe(paymentRequest.User.AsT0.Name);
            //payment.User.Email.ShouldBe(paymentRequest.User.AsT0.Email);
            //payment.User.Phone.ShouldBe(paymentRequest.User.AsT0.Phone);
        }

        private static CreatePaymentRequest CreatePaymentRequest()
            => new CreatePaymentRequest(
                100,
                Currencies.GBP,
                new PaymentMethod.BankTransfer
                {
                    ProviderFilter = new ProviderFilter
                    {
                        ProviderIds = new[] { "mock-payments-gb-redirect" }
                    }
                },
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SchemeIdentifier.SortCodeAccountNumber("567890", "12345678")
                ),
                new PaymentUser.NewUser("Jane Doe", email: "jane.doe@example.com", phone: "+44 1234 567890")
            );
    }
}
