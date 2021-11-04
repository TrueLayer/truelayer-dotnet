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
        private readonly string _baseUri;
        private readonly IAuthApi _auth;
        
        public MerchantsApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _auth = auth.NotNull(nameof(auth));

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            _baseUri = options.Payments.Uri is not null 
                ? new Uri(options.Payments.Uri, "merchant_accounts").AbsoluteUri
                : new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl).AbsoluteUri;
        }
        
        public async Task<ApiResponse<ResourceCollection<MerchantAccount>>> ListMerchants(CancellationToken cancellationToken = default)
        {
            // 'payments' scope should be supported soon
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("paydirect"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }
            
            return await _apiClient.GetAsync<ResourceCollection<MerchantAccount>>(
                new Uri(_baseUri.TrimEnd('/')),
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }
        
        public async Task<ApiResponse<MerchantAccount>> GetMerchant(string merchantId, CancellationToken cancellationToken = default)
        {
            merchantId.NotNullOrWhiteSpace(nameof(merchantId));
            
            // 'payments' scope should be supported soon
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("paydirect"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            var getUri = new Uri(_baseUri.EndsWith('/') ? _baseUri + merchantId : _baseUri + "/" + merchantId);
            return await _apiClient.GetAsync<MerchantAccount>(
                getUri,
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }
    }
}