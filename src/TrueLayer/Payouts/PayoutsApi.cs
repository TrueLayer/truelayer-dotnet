using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Extensions;
using TrueLayer.Payouts.Model;
using static TrueLayer.Payouts.Model.GetPayoutsResponse;

namespace TrueLayer.Payouts
{
    using GetPayoutUnion = OneOf<
        Pending,
        Authorized,
        Successful,
        Failed
    >;

    internal class PayoutsApi : IPayoutsApi
    {
        private const string ProdUrl = "https://api.truelayer.com/payouts";
        private const string SandboxUrl = "https://api.truelayer-sandbox.com/payouts";

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

            string payoutsApiUrl = (options.UseSandbox ?? true) ? SandboxUrl : ProdUrl;
            _baseUri = options.Payments.Uri is not null
                ? new Uri(options.Payments.Uri, "payouts")
                : new Uri(payoutsApiUrl);
        }

        /// <inheritdoc />
        public async Task<ApiResponse<CreatePayoutResponse>> CreatePayout(CreatePayoutRequest payoutRequest, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            payoutRequest.NotNull(nameof(payoutRequest));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("paydirect"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<CreatePayoutResponse>(
                _baseUri,
                payoutRequest,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        public async Task<ApiResponse<GetPayoutUnion>> GetPayout(string id, CancellationToken cancellationToken = default)
        {
            id.NotNullOrWhiteSpace(nameof(id));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("paydirect"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<GetPayoutUnion>(
                _baseUri.Append(id),
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }
    }
}
