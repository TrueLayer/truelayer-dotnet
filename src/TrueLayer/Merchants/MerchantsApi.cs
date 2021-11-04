using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Merchants.Model;

namespace TrueLayer.Merchants
{
    internal class MerchantsApi : IMerchantsApi
    {
        private const string ProdUrl = "https://api.truelayer.com/merchant_accounts";
        private const string SandboxUrl = "https://api.truelayer-sandbox.com/merchant_accounts";
        private readonly IApiClient _apiClient;
        private readonly Uri _baseUri;
        private readonly IAuthApi _auth;
        
        public MerchantsApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _auth = auth.NotNull(nameof(auth));

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            if (options.Payments.Uri is not null)
            {
                var url = options.Payments.Uri.AbsoluteUri.Replace("/payments", "/merchant_accounts").TrimEnd('/');
                var merchantUri = new Uri(url);
                _baseUri = merchantUri;
            }
            else
            {
                _baseUri = new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
            }
        }
        
        public async Task<ApiResponse<ListMerchantsResponse>> ListMerchants(CancellationToken cancellationToken = default)
        {
            // 'payments' scope should be supported soon
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("paydirect"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }
            
            return await _apiClient.GetAsync<ListMerchantsResponse>(
                _baseUri,
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }
    }
}