using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Extensions;
using TrueLayer.Models;
using TrueLayer.Payouts.Model;
using static TrueLayer.Payouts.Model.GetPayoutsResponse;

namespace TrueLayer.Payouts
{
    using GetPayoutUnion = OneOf<
        Pending,
        Authorized,
        Executed,
        Failed
    >;

    internal class PayoutsApi : IPayoutsApi
    {
        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;
        private readonly IAuthApi _auth;

        public PayoutsApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));
            _auth = auth.NotNull(nameof(auth));

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            _baseUri = options.GetApiBaseUri()
                .Append(PayoutsEndpoints.V3Payouts);
        }

        /// <inheritdoc />
        public async Task<ApiResponse<CreatePayoutResponse>> CreatePayout(
            CreatePayoutRequest payoutRequest,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            payoutRequest.NotNull(nameof(payoutRequest));

            var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<CreatePayoutResponse>(
                _baseUri,
                payoutRequest,
                idempotencyKey ?? Guid.NewGuid().ToString(),
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        public async Task<ApiResponse<GetPayoutUnion>> GetPayout(
            string id,
            CancellationToken cancellationToken = default)
        {
            id.NotNullOrWhiteSpace(nameof(id));
            id.NotAUrl(nameof(id));

            var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<GetPayoutUnion>(
                _baseUri.Append(id),
                authResponse.Data!.AccessToken,
                cancellationToken: cancellationToken
            );
        }
    }
}
