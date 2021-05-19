using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrueLayer;
using TrueLayerSdk.SampleApp.Data;
using TrueLayerSdk.SampleApp.Models;

namespace TrueLayerSdk.SampleApp.Pages
{
    public class Callback : PageModel
    {
        public Callback(IConfiguration config, PaymentsDbContext context, ITrueLayerApi api)
        {
            _context = context;
            _api = api;
        }

        private readonly PaymentsDbContext _context;
        public PaymentData Payment;
        private readonly ITrueLayerApi _api;

        public ErrorEntity Error { get; set; }

        public async Task OnGet([FromQuery(Name = "single_immediate_payment_id")] string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
            {
                Error = new ErrorEntity("Payment ID is required", "Payment id is required to fetch status");
                return;
            }

            var result =
                await _api.Payments.GetPayment(Guid.Parse(paymentId), CancellationToken.None);
            var paymentData = result.Result;
            Payment = new PaymentData
            {
                SimpId = paymentData.SingleImmediatePaymentId.ToString(),
                CreatedAt = paymentData.InitiatedAt,
                Status = paymentData.Status,
            };
            var entity = await _context.Payments.FirstAsync(p => p.PaymentEntityId == Payment.SimpId);
            entity.Status = paymentData.Status;
            await _context.SaveChangesAsync();
        }
    }
}
