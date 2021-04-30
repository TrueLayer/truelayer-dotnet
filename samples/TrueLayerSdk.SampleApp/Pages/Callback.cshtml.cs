using System.Linq;
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
        public Callback(IConfiguration config, TokenStorage tokenStorage, PaymentsDbContext context, ITrueLayerApi api)
        {
            _tokenStorage = tokenStorage;
            _context = context;
            _api = api;
            Token = _tokenStorage.AccessToken;
        }
        
        private readonly TokenStorage _tokenStorage;
        private readonly PaymentsDbContext _context;
        public PaymentData Payment;
        private readonly ITrueLayerApi _api;
        
        public string Token { get; }
        public ErrorEntity Error { get; set; }
        
        public async Task OnGet([FromQuery(Name = "payment_id")] string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
            {
                Error = new ErrorEntity("Payment ID is required", "Payment id is required to fetch status");
                return;
            }
            
            if (string.IsNullOrEmpty(_tokenStorage.AccessToken))
            {
                Error = new ErrorEntity("Access token is required", "Access token is required to fetch payment status");
                return;
            }
            
            var result =
                await _api.Payments.GetPaymentStatus(paymentId, _tokenStorage.AccessToken, CancellationToken.None);
            var paymentData = result.Results.First();
            Payment = new PaymentData
            {
                simp_id = paymentData.SimpId,
                created_at = paymentData.CreatedAt,
                status = paymentData.Status,
            };
            var entity = await _context.Payments.FirstAsync(p => p.PaymentEntityId == paymentData.SimpId);
            entity.Status = paymentData.Status;
            await _context.SaveChangesAsync();
        }
    }
}
