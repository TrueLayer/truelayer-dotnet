using System;
using System.Threading.Tasks;
using TrueLayer.Extensions;
using TrueLayer.PaymentsProviders.Model;

namespace TrueLayer.PaymentsProviders
{
    internal class PaymentsProvidersApi : IPaymentsProvidersApi
    {
        private const string ProdUrl = "https://api.truelayer.com/v3/payments-providers/";
        private const string SandboxUrl = "https://api.truelayer-sandbox.com/v3/payments-providers/";

        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;

        public PaymentsProvidersApi(IApiClient apiClient, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            _baseUri = options.Payments.Uri is not null
                ? new Uri(options.Payments.Uri, "/v3/payments-providers/")
                : new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        public async Task<ApiResponse<PaymentsProvider>> GetPaymentsProvider(string id)
        {
            id.NotNullOrWhiteSpace(nameof(id));
            id.NotAUrl(nameof(id));

            UriBuilder baseUri = new(_baseUri.Append(id)) { Query = $"client_id={_options.ClientId}" };

            return await _apiClient.GetAsync<PaymentsProvider>(baseUri.Uri);
        }
    }
}
