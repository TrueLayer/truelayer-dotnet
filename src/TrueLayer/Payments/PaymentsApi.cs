using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.CreatePaymentResponse;

namespace TrueLayer.Payments
{
    internal class PaymentsApi : IPaymentsApi
    {
        internal const string ProdUrl = "https://api.truelayer.com/payments/";
        internal const string SandboxUrl = "https://api.truelayer-sandbox.com/payments/";
        internal static string[] RequiredScopes = new[] { "payments" };
        
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

            options.Validate();

            _baseUri = options.Payments?.Uri ??
                       new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }
        
        public async Task<ApiResponse<OneOf<AuthorizationRequired>>> CreatePayment(CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            paymentRequest.NotNull(nameof(paymentRequest));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new ApiResponse<OneOf<AuthorizationRequired>>(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<OneOf<AuthorizationRequired>>(
                new Uri(_baseUri, "payments"),
                paymentRequest,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        public string CreateHostedPaymentPageLink(string paymentId, string resourceToken, Uri returnUri)
            => _hppLinkBuilder.Build(paymentId, resourceToken, returnUri);
    }
}
