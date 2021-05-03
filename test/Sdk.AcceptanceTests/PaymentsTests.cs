using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Auth.Model;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    using System;

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
            var authResponse = await _fixture.Api.Auth.GetPaymentToken();
            authResponse.ShouldNotBeNull();
            authResponse.AccessToken.ShouldNotBeNullOrEmpty();
            authResponse.ExpiresIn.ShouldBeGreaterThan(0);

            var req = MockPaymentRequestData();

            var paymentResponse = await _fixture.Api.Payments.InitiatePayment(req, authResponse.AccessToken);
            paymentResponse.ShouldNotBeNull();
            paymentResponse.Result.ShouldNotBeNull();
            paymentResponse.Result.SingleImmediatePayment.ShouldNotBeNull();
            paymentResponse.Result.SingleImmediatePayment.SingleImmediatePaymentId.ShouldNotBe(Guid.Empty);
            paymentResponse.Result.AuthFlow.ShouldNotBeNull();
            //paymentResponse.Result.AuthFlow.Uri.ShouldNotBeNullOrEmpty();
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
