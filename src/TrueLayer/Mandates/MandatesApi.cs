using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth;

namespace TrueLayer.Mandates
{
    using TrueLayer.Mandates.Model;

    internal class MandatesApi : IMandatesApi
    {
        private const string ProdUrl = "https://api.truelayer.com/v3/mandates";
        private const string SandboxUrl = "https://api.truelayer-sandbox.com/v3/mandates";

        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;
        private readonly IAuthApi _auth;

        public MandatesApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));
            _auth = auth.NotNull(nameof(auth));

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            _baseUri = options.Payments.Uri is not null
                ? new Uri(options.Payments.Uri, "mandates")
                : new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        /// <inheritdoc />
        public async Task<ApiResponse<CreateMandateResponse>> CreateMandate(CreateMandateRequest mandateRequest, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            mandateRequest.NotNull(nameof(mandateRequest));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("recurring_payments:sweeping"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            var response = await _apiClient.PostAsync<CreateMandateResponse>(
                _baseUri,
                mandateRequest,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
            if (!response.IsSuccessful)
            {
                throw new Exception($"{response.StatusCode} - {response.TraceId} - {response.Data}}}");
            }

            return response;
        }


        /// <inheritdoc />
        public async Task<ApiResponse<Task>> RevokeMandate(string id, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            id.NotNullOrWhiteSpace(nameof(id));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<Task>(
                new Uri(_baseUri, $"mandates/{id}/revoke"),
                null,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }
    }
}
