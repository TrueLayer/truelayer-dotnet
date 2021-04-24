using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public string Position;
        [BindProperty]
        public PaymentData Payment { get; set; }
        
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
                amount = Payment.amount,
                remitter_provider_id = Payment.remitter_provider_id,
                remitter_name = Payment.remitter_name,
                remitter_sort_code = Payment.remitter_sort_code,
                remitter_account_number = Payment.remitter_account_number,
                remitter_reference = Payment.remitter_reference,
                beneficiary_name = Payment.beneficiary_name,
                beneficiary_sort_code = Payment.beneficiary_sort_code,
                beneficiary_account_number = Payment.beneficiary_account_number,
                beneficiary_reference = Payment.beneficiary_reference,
            };
            var result = await _api.Payments.SingleImmediatePayment(request, CancellationToken.None);
            PaymentId = result.results.First().simp_id;
            AuthUri = result.results.First().auth_uri;
            Position = "auth_uri";
        }
    }

    public class PaymentData
    {
        public int amount { get; set; }
        public string remitter_provider_id { get; set; }
        public string remitter_name { get; set; }
        public string remitter_sort_code { get; set; }
        public string remitter_account_number { get; set; }
        public string remitter_reference { get; set; }
        public string beneficiary_name { get; set; }
        public string beneficiary_sort_code { get; set; }
        public string beneficiary_account_number { get; set; }
        public string beneficiary_reference { get; set; }
        
        public string simp_id { get; set; }
        public DateTime created_at { get; set; }
        public string status { get; set; }

    }
}
