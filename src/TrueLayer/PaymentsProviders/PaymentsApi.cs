using System;
using System.Threading.Tasks;
using TrueLayer.Common;
using TrueLayer.Extensions;
using TrueLayer.PaymentsProviders.Model;

namespace TrueLayer.PaymentsProviders
{
    internal class PaymentsProvidersApi : IPaymentsProvidersApi
    {
        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;

        public PaymentsProvidersApi(IApiClient apiClient, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            var baseUri = (options.UseSandbox ?? true)
                ? TrueLayerBaseUris.SandboxApiBaseUri
                : TrueLayerBaseUris.ProdApiBaseUri;

            _baseUri = (options.Payments.Uri ?? baseUri)
                .Append("/v3/payments-providers/");
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
