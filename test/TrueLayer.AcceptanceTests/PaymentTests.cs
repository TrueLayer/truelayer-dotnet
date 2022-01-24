using System;
using System.Net;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

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
            response.Data.ShouldBeOfType<CreatePaymentResponse>();

            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldNotBeNullOrWhiteSpace();
            response.Data.PaymentToken.ShouldNotBeNullOrWhiteSpace();
            response.Data.User.ShouldNotBeNull();
            response.Data.User.Id.ShouldNotBeNullOrWhiteSpace();

            string hppUri = _fixture.Client.Payments.CreateHostedPaymentPageLink(
                response.Data!.Id, response.Data!.PaymentToken, new Uri("https://redirect.mydomain.com"));
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
                = await _fixture.Client.Payments.GetPayment(response.Data!.Id);

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
            payment.User.Id.ShouldBe(response.Data.User.Id);
            payment.User.Name.ShouldBe(paymentRequest.User!.Name);
            payment.User.Email.ShouldBe(paymentRequest.User!.Email);
            payment.User.Phone.ShouldBe(paymentRequest.User!.Phone);
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

                new PaymentUserRequest("Jane Doe", "jane.doe@example.com", "+44 1234 567890")
            );
    }
}
