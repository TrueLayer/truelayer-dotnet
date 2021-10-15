using System;
using System.Collections.Generic;
using System.Linq;
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
            var paymentRequest = new CreatePaymentRequest(
                100,
                Currencies.GBP,
                PaymentMethod.BankTransfer(providerFilter: new ProviderFilter
                {
                    ProviderIds = new[] { "mock-payments-gb-redirect" }
                }),
                Beneficiary.ToExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    SchemeIdentifier.SortCodeAccountNumber("567890", "12345678")
                )
            );

            ApiResponse<CreatePaymentResponse> response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, Guid.NewGuid().ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Data.ShouldNotBeNull();
            response.Data.Status.ShouldBe("authorization_required");
        }
    }
}
