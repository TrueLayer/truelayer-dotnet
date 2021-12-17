using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;
using TrueLayer.Payouts.Model;

namespace TrueLayer.Payouts
{
    internal class PayoutsApi : IPayoutsApi
    {
        private const string ProdUrl = "https://api.truelayer.com/payouts/";
        private const string SandboxUrl = "https://api.truelayer-sandbox.com/payouts/";

        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;
        private readonly IAuthApi _auth;

        public PayoutsApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));
            _auth = auth.NotNull(nameof(auth));

            options.Payouts.NotNull(nameof(options.Payouts))!.Validate();

            _baseUri = options.Payouts.Uri is not null
                ? new Uri(options.Payouts.Uri, "payouts/")
                : new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
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
                _options.Payouts!.SigningKey,
                cancellationToken
            );
        }
    }
}
