using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrueLayerSdk.SampleApp.Data;
using TrueLayerSdk.SampleApp.Models;

namespace TrueLayerSdk.SampleApp.Pages
{
    public class Callback : PageModel
    {
        public Callback(IConfiguration config, TokenStorage tokenStorage, PaymentsDbContext context, ITruelayerApi api)
        {
            _tokenStorage = tokenStorage;
            _context = context;
            _api = api;
            Token = _tokenStorage.AccessToken;
        }
        
        private readonly TokenStorage _tokenStorage;
        private readonly PaymentsDbContext _context;
        public PaymentData Payment;
        private readonly ITruelayerApi _api;
        
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
            var paymentData = result.results.First();
            Payment = new PaymentData
            {
                simp_id = paymentData.simp_id,
                created_at = paymentData.created_at,
                status = paymentData.status,
            };
            var entity = await _context.Payments.FirstAsync(p => p.PaymentEntityId == paymentData.simp_id);
            entity.Status = paymentData.status;
            await _context.SaveChangesAsync();
        }
    }
}
