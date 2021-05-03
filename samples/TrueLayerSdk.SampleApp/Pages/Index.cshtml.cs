using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrueLayer;
using TrueLayer.Auth.Model;
using TrueLayer.Payments.Model;
using TrueLayerSdk.SampleApp.Data;
using TrueLayerSdk.SampleApp.Models;

namespace TrueLayerSdk.SampleApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly TokenStorage _tokenStorage;
        private readonly PaymentsDbContext _context;
        private readonly ITrueLayerApi _api;
        public string Token;
        public string PaymentId;
        public string AuthUri;
        public string Position;
        [BindProperty]
        public PaymentData Payment { get; set; }
        
        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, TokenStorage tokenStorage,
            PaymentsDbContext context, ITrueLayerApi api)
        {
            _logger = logger;
            _tokenStorage = tokenStorage;
            _context = context;
            _api = api;
            Token = _tokenStorage.AccessToken;
        }

        public void OnGet()
        {
        }

        public async Task OnGetToken()
        {
            var result = await _api.Auth.GetPaymentToken();
            _tokenStorage.SetToken(result.AccessToken, result.ExpiresIn);
            Token = _tokenStorage.AccessToken;
        }

        public async Task OnPost()
        {
            // var request = new SingleImmediatePaymentRequest
            // {
            //     AccessToken = _tokenStorage.AccessToken,
            //     Data = new SingleImmediatePaymentData
            //     {
            //         RedirectUri = "https://localhost:5001/callback",
            //         Amount = Payment.amount * 100,
            //         Currency = "GBP",
            //         RemitterProviderId = Payment.remitter_provider_id,
            //         RemitterName = Payment.remitter_name,
            //         RemitterSortCode = Payment.remitter_sort_code,
            //         RemitterAccountNumber = Payment.remitter_account_number,
            //         RemitterReference = Payment.remitter_reference,
            //         BeneficiaryName = Payment.beneficiary_name,
            //         BeneficiarySortCode = Payment.beneficiary_sort_code,
            //         BeneficiaryAccountNumber = Payment.beneficiary_account_number,
            //         BeneficiaryReference = Payment.beneficiary_reference,
            //     },
            // };
            // var result = await _api.Payments.SingleImmediatePayment(request);
            // PaymentId = result.Results.First().SimpId;
            // AuthUri = result.Results.First().AuthUri;
            // Position = "auth_uri";
            // await _context.Payments.AddAsync(new PaymentEntity
            //     {PaymentEntityId = PaymentId, CreatedAt = DateTime.UtcNow, Status = result.Results.First().Status});
            // await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }
    }
}
