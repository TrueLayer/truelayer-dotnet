using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TrueLayer;
using TrueLayer.Payments.Model;
using TrueLayerSdk.SampleApp.Data;
using TrueLayerSdk.SampleApp.Models;
using System.Threading;

namespace TrueLayerSdk.SampleApp.Pages.Payments
{
    public class NewPayment : PageModel
    {
        private readonly PaymentsDbContext _context;
        private readonly ITrueLayerApi _api;
        public string PaymentId;
        public string AuthUri;
        public string Position;

        [BindProperty]
        public PaymentData Payment { get; set; }
        
        public NewPayment(IConfiguration config, PaymentsDbContext context, ITrueLayerApi api)
        {
            _context = context;
            _api = api;
        }

        public async Task OnPost()
        {
            var benAccount = new Account
            {
                AccountNumber = Payment.BeneficiaryAccountNumber,
                SortCode = Payment.BeneficiarySortCode,
                Type = "sort_code_account_number",
            };
            var remAccount = new Account
            {
                AccountNumber = Payment.RemitterAccountNumber,
                SortCode = Payment.RemitterSortCode,
                Type = "sort_code_account_number",
            };
            var beneficiary = new Beneficiary(benAccount) {Name = Payment.BeneficiaryName};
            var payment = new SingleImmediatePayment(Payment.Amount * 100, "GBP", Payment.RemitterProviderId,
                "faster_payments_service", beneficiary, Guid.NewGuid())
            {
                Remitter = new Remitter(remAccount)
                {
                    Name = Payment.RemitterName,
                },
                References = new References
                {
                    Beneficiary = Payment.RemitterReference,
                    Remitter = Payment.BeneficiaryReference,
                    Type = "separate",
                },
            };
            var authFlow = new AuthFlow("redirect") {ReturnUri = "https://localhost:5001/callback"};
            var request = new InitiatePaymentRequest(payment, authFlow);
            var result = await _api.Payments.InitiatePayment(request, CancellationToken.None);
            PaymentId = result.Result.SingleImmediatePayment.SingleImmediatePaymentId.ToString();
            AuthUri = result.Result.AuthFlow.Uri;
            Position = "auth_uri";
            await _context.Payments.AddAsync(new PaymentEntity
                {PaymentEntityId = PaymentId, CreatedAt = DateTime.UtcNow, Status = result.Result.SingleImmediatePayment.Status});
            await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }
    }
}
