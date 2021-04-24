using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrueLayerSdk.Auth.Models;
using TrueLayerSdk.Payments.Models;

namespace TrueLayerSdk.SampleApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly TokenStorage _tokenStorage;
        private readonly TruelayerApi _api;
        public string Token;
        public string PaymentId;
        public string AuthUri;
        
        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, TokenStorage tokenStorage)
        {
            _logger = logger;
            _tokenStorage = tokenStorage;
            _api = TruelayerApi.Create(config["clientId"], config["clientSecret"], true);
            Token = _tokenStorage.AccessToken;
        }

        public void OnGet()
        {
        }

        public async Task OnGetToken()
        {
            var result = await _api.Auth.GetPaymentToken(new GetPaymentTokenRequest(), CancellationToken.None);
            _tokenStorage.SetToken(result.AccessToken, result.ExpiresIn);
            Token = _tokenStorage.AccessToken;
        }

        public async Task OnPost()
        {
            var request = new SingleImmediatePaymentRequest
            {
                AccessToken = _tokenStorage.AccessToken,
                ReturnUri = "https://localhost:5001/callback",
            };
            var result = await _api.Payments.SingleImmediatePayment(request, CancellationToken.None);
            PaymentId = result.results.First().simp_id;
            AuthUri = result.results.First().auth_uri;
        }
    }
}
