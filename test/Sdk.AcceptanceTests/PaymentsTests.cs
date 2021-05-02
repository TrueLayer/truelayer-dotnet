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
            var response = await _fixture.Api.Auth.GetPaymentToken(new GetPaymentTokenRequest());
            response.ShouldNotBeNull();
            response.AccessToken.ShouldNotBeNullOrEmpty();
            response.ExpiresIn.ShouldBeGreaterThan(0);

            var req = new SingleImmediatePaymentInitiationRequest
            {
                AccessToken = response.AccessToken, Data = MockPaymentRequestData()
            };
            var resp = await _fixture.Api.Payments.SingleImmediatePaymentInitiation(req);
            resp.ShouldNotBeNull();
            resp.Result.ShouldNotBeNull();
            resp.Result.SingleImmediatePayment.ShouldNotBeNull();
            resp.Result.SingleImmediatePayment.SingleImmediatePaymentId.ShouldNotBeNullOrEmpty();
            resp.Result.AuthFlow.ShouldNotBeNull();
            resp.Result.AuthFlow.Uri.ShouldNotBeNullOrEmpty();
        }

        private static SingleImmediatePaymentInitiationData MockPaymentRequestData()
        {
            return new()
            {
                SingleImmediatePayment = new SingleImmediatePayment
                {
                    SingleImmediatePaymentId = Guid.NewGuid().ToString(),
                    ProviderId = "ob-sandbox-natwest",
                    SchemeId = "faster_payments_service",
                    // FeeOptionId = "free",
                    AmountInMinor = 100,
                    Currency = "GBP",
                    Beneficiary = new Beneficiary
                    {
                        Name = "A lucky someone",
                        Account = new Account
                        {
                            Type = "sort_code_account_number",
                            AccountNumber = "12345678",
                            SortCode = "567890",
                        },
                    },
                    Remitter = new Remitter
                    {
                        Name = "A less lucky someone",
                        Account = new Account
                        {
                            Type = "sort_code_account_number",
                            AccountNumber = "12345602",
                            SortCode = "500000",
                        },
                    },
                    References = new References
                    {
                        Type = "separate",
                        Beneficiary = "beneficiary ref",
                        Remitter = "remitter ref",
                    },
                },
                AuthFlow = new AuthFlow {Type = "redirect", ReturnUri = "https://localhost:5001/home/callback"},
            };
        }
    }
}
