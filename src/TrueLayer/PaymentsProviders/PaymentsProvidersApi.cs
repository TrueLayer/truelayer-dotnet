using System;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Extensions;
using TrueLayer.Models;
using TrueLayer.PaymentsProviders.Model;

namespace TrueLayer.PaymentsProviders;

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

        _baseUri = options.GetApiBaseUri()
            .Append(PaymentsProvidersEndpoints.V3PaymentsProviders);
    }

    public async Task<ApiResponse<PaymentsProvider>> GetPaymentsProvider(string id)
    {
        id.NotNullOrWhiteSpace(nameof(id));
        id.NotAUrl(nameof(id));

        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments));

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

    public async Task<ApiResponse<SearchPaymentsProvidersResponse>> SearchPaymentsProviders(SearchPaymentsProvidersRequest searchPaymentsProvidersRequest)
    {
        searchPaymentsProvidersRequest.NotNull(nameof(searchPaymentsProvidersRequest));

        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments));

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.PostAsync<SearchPaymentsProvidersResponse>(
            _baseUri.Append(PaymentsProvidersEndpoints.Search),
            request: searchPaymentsProvidersRequest,
            accessToken: authResponse.Data!.AccessToken
        );
    }
}