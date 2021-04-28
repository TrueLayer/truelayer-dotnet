using System.Threading.Tasks;
using Shouldly;
using TrueLayerSdk.Auth.Models;
using TrueLayerSdk.Payments.Models;
using Xunit;

namespace TrueLayer.Acceptance.Tests
{
    public class PaymentsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        
        public PaymentsTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_initiate_payment()
        {
            var response = await _fixture.Api.Auth.GetPaymentToken(new GetPaymentTokenRequest());
            response.ShouldNotBeNull();
            response.AccessToken.ShouldNotBeNullOrEmpty();
            response.ExpiresIn.ShouldBeGreaterThan(0);

            var req = new SingleImmediatePaymentInitiationRequest
            {
                AccessToken = response.AccessToken, ReturnUri = "https://localhost:5001/home/callback"
            };
            var resp = await _fixture.Api.Payments.SingleImmediatePaymentInitiation(req);
            resp.ShouldNotBeNull();
            resp.Result.ShouldNotBeNull();
            resp.Result.SingleImmediatePayment.ShouldNotBeNull();
            resp.Result.SingleImmediatePayment.SingleImmediatePaymentId.ShouldNotBeNullOrEmpty();
        }
    }
}
