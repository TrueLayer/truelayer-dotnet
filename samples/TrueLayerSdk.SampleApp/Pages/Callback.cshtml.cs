using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TrueLayerSdk.SampleApp.Models;

namespace TrueLayerSdk.SampleApp.Pages
{
    public class Callback : PageModel
    {
        public Callback(IConfiguration config, TokenStorage tokenStorage)
        {
            _tokenStorage = tokenStorage;
            _api = TruelayerApi.Create(config["clientId"], config["clientSecret"], true);
            Token = _tokenStorage.AccessToken;
        }
        
        private readonly TokenStorage _tokenStorage;
        public PaymentData Payment;
        private readonly TruelayerApi _api;
        
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
        }
    }
}
