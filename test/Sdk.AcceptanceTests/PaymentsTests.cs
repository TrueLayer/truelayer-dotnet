using System;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Sdk.Acceptance.Tests
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
            // ARRANGE
            var req = MockPaymentRequestData();

            // ACT
            var paymentResponse = await _fixture.Api.Payments.InitiatePayment(req);
            
            // ASSERT
            paymentResponse.ShouldNotBeNull();
            paymentResponse.Result.ShouldNotBeNull();
            paymentResponse.Result.SingleImmediatePayment.ShouldNotBeNull();
            paymentResponse.Result.SingleImmediatePayment.SingleImmediatePaymentId.ShouldNotBe(Guid.Empty);
            paymentResponse.Result.AuthFlow.ShouldNotBeNull();
        }

        [Fact]
        public async Task Can_fetch_payment_token()
        {
            // ACT
            var authResponse = await _fixture.Api.Auth.GetPaymentToken();
            
            // ASSERT
            authResponse.ShouldNotBeNull();
            authResponse.AccessToken.ShouldNotBeNullOrEmpty();
            authResponse.ExpiresIn.ShouldBeGreaterThan(0);
        }
        
        private static InitiatePaymentRequest MockPaymentRequestData()
        {
            return new(
                new SingleImmediatePayment(
                    amountInMinor: 100,
                    currency: "GBP",
                    providerId: "ob-sandbox-natwest",
                    schemeId: "faster_payments_service",
                    beneficiary: new Beneficiary(
                        new Account
                        {
                            Type = "sort_code_account_number",
                            AccountNumber = "12345678",
                            SortCode = "567890"
                        }
                    )
                    {
                        Name = "A lucky someone"
                    }
                )
                {
                    Remitter = new Remitter(
                        new Account
                        {
                            Type = "sort_code_account_number",
                            AccountNumber = "12345602",
                            SortCode = "500000",
                        }
                    )
                    {
                        Name = "A less lucky someone"
                    },
                    References = new References
                    {
                        Type = "separate",
                        Beneficiary = "beneficiary ref",
                        Remitter = "remitter ref",
                    },
                },
                new AuthFlow(type: "redirect")
                {
                    ReturnUri = "https://localhost:5001/home/callback"
                }
            );
        }
    }
}
