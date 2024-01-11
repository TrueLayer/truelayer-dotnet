using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Payments.Model;

namespace TrueLayer.Payments
{
    using CreatePaymentUnion = OneOf<
        CreatePaymentResponse.AuthorizationRequired,
        CreatePaymentResponse.Authorized,
        CreatePaymentResponse.Failed
    >;

    using GetPaymentUnion = OneOf<
        GetPaymentResponse.AuthorizationRequired,
        GetPaymentResponse.Authorizing,
        GetPaymentResponse.Authorized,
        GetPaymentResponse.Executed,
        GetPaymentResponse.Settled,
        GetPaymentResponse.Failed
    >;

    internal class PaymentsApi : IPaymentsApi
    {
        private const string ProdUrl = "https://api.truelayer.com/v3/payments/";
        private const string SandboxUrl = "https://api.truelayer-sandbox.com/v3/payments/";

        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;
        private readonly IAuthApi _auth;
        private readonly HppLinkBuilder _hppLinkBuilder;

        public PaymentsApi(IApiClient apiClient, IAuthApi auth, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options));
            _auth = auth.NotNull(nameof(auth));
            _hppLinkBuilder = new HppLinkBuilder(options.Payments?.HppUri, options.UseSandbox ?? true);

            options.Payments.NotNull(nameof(options.Payments))!.Validate();

            var baseUri = (options.UseSandbox ?? true) ? SandboxUrl : ProdUrl;
            _baseUri = options.Payments.Uri is not null
                ? new Uri(options.Payments.Uri, "/v3/payments/")
                : new Uri(baseUri);
        }

        /// <inheritdoc />
        public async Task<ApiResponse<CreatePaymentUnion>> CreatePayment(CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            paymentRequest.NotNull(nameof(paymentRequest));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<CreatePaymentUnion>(
                _baseUri,
                paymentRequest,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }


        /// <inheritdoc />
        public async Task<ApiResponse<GetPaymentUnion>> GetPayment(string id, CancellationToken cancellationToken = default)
        {
            id.NotNullOrWhiteSpace(nameof(id));
            id.NotAnUrl(nameof(id));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<GetPaymentUnion>(
                new Uri(_baseUri, id),
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public string CreateHostedPaymentPageLink(string paymentId, string paymentToken, Uri returnUri)
            => _hppLinkBuilder.Build(paymentId, paymentToken, returnUri);
    }
}
