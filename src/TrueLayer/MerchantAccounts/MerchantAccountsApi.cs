using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Extensions;
using TrueLayer.MerchantAccounts.Model;
using TrueLayer.Models;

namespace TrueLayer.MerchantAccounts;

internal class MerchantAccountsApi : IMerchantAccountsApi
{
    private readonly IApiClient _apiClient;
    private readonly Uri _baseUri;
    private readonly IAuthApi _auth;
    private const bool UsePagination = true;

    public MerchantAccountsApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
    {
        _apiClient = apiClient.NotNull(nameof(apiClient));
        _auth = auth.NotNull(nameof(auth));

        options.Payments.NotNull(nameof(options.Payments))!.Validate();

        _baseUri = _baseUri = options.GetApiBaseUri()
            .Append(MerchantAccountsEndpoints.V3MerchantAccounts);
    }

    /// <inheritdoc />
    public async Task<ApiResponse<ResourceCollection<MerchantAccount>>> ListMerchantAccounts(CancellationToken cancellationToken = default)
    {
        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.GetAsync<ResourceCollection<MerchantAccount>>(
            _baseUri,
            authResponse.Data!.AccessToken,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResponse<MerchantAccount>> GetMerchantAccount(string id, CancellationToken cancellationToken = default)
    {
        id.NotNullOrWhiteSpace(nameof(id));
        id.NotAUrl(nameof(id));

        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.GetAsync<MerchantAccount>(
            _baseUri.Append(id),
            authResponse.Data!.AccessToken,
            cancellationToken:cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResponse<GetPaymentSourcesResponse>> GetPaymentSources(string merchantAccountId, string userId, CancellationToken cancellationToken = default)
    {
        merchantAccountId.NotNullOrWhiteSpace(nameof(merchantAccountId));
        merchantAccountId.NotAUrl(nameof(merchantAccountId));
        userId.NotNullOrWhiteSpace(nameof(userId));
        userId.NotAUrl(nameof(userId));

        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.GetAsync<GetPaymentSourcesResponse>(
            _baseUri.Append(merchantAccountId).Append($"/payment-sources?user_id={userId}"),
            authResponse.Data!.AccessToken,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<ApiResponse<GetTransactionsResponse>> GetTransactions(
        string merchantAccountId,
        DateTimeOffset from,
        DateTimeOffset to,
        string? cursor = null,
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        merchantAccountId.NotNullOrWhiteSpace(nameof(merchantAccountId));
        merchantAccountId.NotAUrl(nameof(merchantAccountId));
        to.GreaterThan(from, nameof(to), nameof(from));
        cursor.NotAUrl(nameof(cursor));

        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        var customHeaders = new Dictionary<string, string>
        {
            ["tl-use-pagination"] = UsePagination.ToString()
        };

        return await _apiClient.GetAsync<GetTransactionsResponse>(
            _baseUri.Append(merchantAccountId).Append(MerchantAccountsEndpoints.Transactions)
                .AppendQueryParameters(new Dictionary<string, string?>
                {
                    ["from"] = from.ToString("yyyy-MM-ddTHH:MM:ssZ"),
                    ["to"] = to.ToString("yyyy-MM-ddTHH:MM:ssZ"),
                    ["cursor"] = cursor,
                    ["type"] = type
                }),
            authResponse.Data!.AccessToken,
            customHeaders,
            cancellationToken
        );
    }
}