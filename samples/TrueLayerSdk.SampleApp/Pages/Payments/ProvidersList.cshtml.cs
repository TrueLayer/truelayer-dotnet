using Microsoft.AspNetCore.Mvc.RazorPages;
using TrueLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

namespace TrueLayerSdk.SampleApp.Pages.Payments
{
    public class ProvidersList : PageModel
    {
        private readonly ITrueLayerApi _api;
        private readonly TrueLayerOptions _options;

        public List<Result> Providers { get; private set; }
        
        public ProvidersList(ITrueLayerApi api, TrueLayerOptions options)
        {
            _api = api;
            _options = options;
        }

        public async Task OnGet()
        {
            var request = new GetProvidersRequest(
                _options.ClientId ?? throw new ArgumentNullException(nameof(_options.ClientId)),
                new List<string> {"redirect", "embedded"},
                new List<string> {"sort_code_account_number", "iban"},
                new List<string> {"GBP", "EUR"});
            var response = await _api.Payments.GetProviders(request);
            Providers = response.Results;
        }
    }
}
