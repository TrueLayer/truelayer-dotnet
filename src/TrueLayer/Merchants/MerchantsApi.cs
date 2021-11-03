using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Merchants.Model;

namespace TrueLayer.Merchants
{
    internal class MerchantsApi : IMerchantsApi
    {
        private const string Url = "https://test-pay-api.t7r.dev/merchant_accounts/";
        private readonly IApiClient _apiClient;
        private readonly Uri _baseUri;
        private readonly IAuthApi _auth;
        
        public MerchantsApi(IApiClient apiClient, IAuthApi auth)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _auth = auth.NotNull(nameof(auth));

            _baseUri = new Uri(Url);
        }
        
        public async Task<ApiResponse<ListMerchantsResponse>> ListMerchants(CancellationToken cancellationToken = default)
        {
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("payments"), cancellationToken);

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