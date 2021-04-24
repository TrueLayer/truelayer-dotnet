using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

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
        
        public string Token { get; set; }

        public async Task OnGet([FromQuery] string payment_id)
        {
            var result =
                await _api.Payments.GetPaymentStatus(payment_id, _tokenStorage.AccessToken, CancellationToken.None);
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
