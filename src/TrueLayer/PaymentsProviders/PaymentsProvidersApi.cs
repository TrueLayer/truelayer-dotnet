using System;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Common;
using TrueLayer.Extensions;
using TrueLayer.PaymentsProviders.Model;

namespace TrueLayer.PaymentsProviders
{
    internal class PaymentsProvidersApi : IPaymentsProvidersApi
    {
        private readonly IApiClient _apiClient;
        private readonly IAuthApi _auth;
        private readonly Uri _baseUri;

        public PaymentsProvidersApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _auth = auth.NotNull(nameof(auth));

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

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("payments"));

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            UriBuilder baseUri = new(_baseUri.Append(id));

            return await _apiClient.GetAsync<PaymentsProvider>(
                baseUri.Uri,
                accessToken: authResponse.Data!.AccessToken
            );
        }
    }
}
