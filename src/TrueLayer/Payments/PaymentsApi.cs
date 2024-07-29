using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Common;
using TrueLayer.Extensions;
using TrueLayer.Payments.Model;
using TrueLayer.Payments.Model.AuthorizationFlow;

namespace TrueLayer.Payments
{
    using AuthorizationResponseUnion = OneOf<
        AuthorizationFlowResponse.AuthorizationFlowAuthorizing,
        AuthorizationFlowResponse.AuthorizationFlowAuthorizationFailed
    >;
    using CreatePaymentUnion = OneOf<
        CreatePaymentResponse.AuthorizationRequired,
        CreatePaymentResponse.Authorized,
        CreatePaymentResponse.Failed,
        CreatePaymentResponse.Authorizing
    >;
    using GetPaymentUnion = OneOf<
        GetPaymentResponse.AuthorizationRequired,
        GetPaymentResponse.Authorizing,
        GetPaymentResponse.Authorized,
        GetPaymentResponse.Executed,
        GetPaymentResponse.Settled,
        GetPaymentResponse.Failed
    >;

    using RefundUnion = OneOf<RefundPending, RefundAuthorized>;

    internal class PaymentsApi : IPaymentsApi
    {
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

            var baseUri = (options.UseSandbox ?? true)
                ? TrueLayerBaseUris.SandboxApiBaseUri
                : TrueLayerBaseUris.ProdApiBaseUri;

            _baseUri = (options.Payments.Uri ?? baseUri)
                .Append("/v3/payments/");
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
            id.NotAUrl(nameof(id));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<GetPaymentUnion>(
                _baseUri.Append(id),
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public string CreateHostedPaymentPageLink(string paymentId, string paymentToken, Uri returnUri)
            => _hppLinkBuilder.Build(paymentId, paymentToken, returnUri);

        /// <inheritdoc />
        public async Task<ApiResponse<AuthorizationResponseUnion>> StartAuthorizationFlow(
            string paymentId,
            string idempotencyKey,
            StartAuthorizationFlowRequest request,
            CancellationToken cancellationToken = default)
        {
            paymentId.NotNullOrWhiteSpace(nameof(paymentId));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));
            request.NotNull(nameof(request));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(
                new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<AuthorizationResponseUnion>(
                _baseUri.Append(paymentId).Append("/authorization-flow"),
                request,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        public async Task<ApiResponse<CreatePaymentRefundResponse>> CreatePaymentRefund(string paymentId,
            string idempotencyKey, CreatePaymentRefundRequest request, CancellationToken cancellationToken = default)
        {
            paymentId.NotNullOrWhiteSpace(nameof(paymentId));
            request.NotNull(nameof(request));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(
                new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<CreatePaymentRefundResponse>(
                _baseUri.Append(paymentId).Append("/refunds"),
                request,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        public async Task<ApiResponse<ListPaymentRefundsResponse>> ListPaymentRefunds(string paymentId,
            CancellationToken cancellationToken = default)
        {
            paymentId.NotNullOrWhiteSpace(nameof(paymentId));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(
                new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<ListPaymentRefundsResponse>(
                _baseUri.Append(paymentId).Append("/refunds"),
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }

        public async Task<ApiResponse<RefundUnion>> GetPaymentRefund(string paymentId,
            string refundId, CancellationToken cancellationToken = default)
        {
            paymentId.NotNullOrWhiteSpace(nameof(paymentId));
            refundId.NotNullOrWhiteSpace(nameof(refundId));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(
                new GetAuthTokenRequest("payments"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<RefundUnion>(
                _baseUri.Append(paymentId).Append("/refunds/").Append(refundId),
                authResponse.Data!.AccessToken,
                cancellationToken
            );
        }
    }
}
