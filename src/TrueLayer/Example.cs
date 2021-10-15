using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueLayer.Payments;
using TrueLayer.Payments.Model;

namespace TrueLayer
{
    public class Example
    {
        private IPaymentsApi _client = (IPaymentsApi)new object();
        
        public async Task Go()
        {
            var paymentRequest = new CreatePaymentRequest(
                100,
                Currencies.GBP,
                PaymentMethod.BankTransfer(statementReference: "ASOS"),
                Beneficiary.ToExternalAccount(
                    "Ben Foster", 
                    "ORD-1234", 
                    SchemeIdentifier.SortCodeAccountNumber("123456", "123456789")
                )
            );

            ApiResponse<CreatePaymentResponse> response = await _client.CreatePayment(paymentRequest, "idempotency-key");

            if (response.Data is CreatePaymentResponse.AuthorizationRequired authRequired)
            {
                var resourceToken = authRequired.ResourceToken;
                return;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
