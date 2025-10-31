using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Extensions;
using TrueLayer.Models;
using TrueLayer.Payments.Model;
using TrueLayer.Payments.Model.AuthorizationFlow;
using StartAuthorizationFlowRequest = TrueLayer.Payments.Model.AuthorizationFlow.StartAuthorizationFlowRequest;

namespace TrueLayer.Payments;

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
    GetPaymentResponse.Failed,
    GetPaymentResponse.AttemptFailed
>;

using RefundUnion = OneOf<RefundPending, RefundAuthorized, RefundExecuted, RefundFailed>;

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

        _baseUri = options.GetApiBaseUri()
            .Append(PaymentsEndpoints.V3Payments);
    }

    /// <inheritdoc />
    public async Task<ApiResponse<CreatePaymentUnion>> CreatePayment(CreatePaymentRequest paymentRequest, string? idempotencyKey = null, CancellationToken cancellationToken = default)
    {
        paymentRequest.NotNull(nameof(paymentRequest));

        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.PostAsync<CreatePaymentUnion>(
            _baseUri,
            paymentRequest,
            idempotencyKey ?? Guid.NewGuid().ToString(),
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

        var authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.GetAsync<GetPaymentUnion>(
            _baseUri.Append(id),
            authResponse.Data!.AccessToken,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public string CreateHostedPaymentPageLink(string paymentId, string paymentToken, Uri returnUri)
        => _hppLinkBuilder.Build(paymentId, paymentToken, returnUri);

    /// <inheritdoc />
    public async Task<ApiResponse<AuthorizationResponseUnion>> StartAuthorizationFlow(
        string paymentId,
        string? idempotencyKey,
        StartAuthorizationFlowRequest request,
        CancellationToken cancellationToken = default)
    {
        paymentId.NotNullOrWhiteSpace(nameof(paymentId));

        paymentId.NotAUrl(nameof(paymentId));
        idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));
            
        request.NotNull(nameof(request));

        var authResponse = await _auth.GetAuthToken(
            new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.PostAsync<AuthorizationResponseUnion>(
            _baseUri.Append(paymentId).Append(PaymentsEndpoints.AuthorizationFlow),
            request,
            idempotencyKey ?? Guid.NewGuid().ToString(),
            authResponse.Data!.AccessToken,
            _options.Payments!.SigningKey,
            cancellationToken
        );
    }

    public async Task<ApiResponse<CreatePaymentRefundResponse>> CreatePaymentRefund(
        string paymentId,
        string? idempotencyKey,
        CreatePaymentRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        paymentId.NotNullOrWhiteSpace(nameof(paymentId));
        paymentId.NotAUrl(nameof(paymentId));
        request.NotNull(nameof(request));

        var authResponse = await _auth.GetAuthToken(
            new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.PostAsync<CreatePaymentRefundResponse>(
            _baseUri.Append(paymentId).Append(PaymentsEndpoints.Refunds),
            request,
            idempotencyKey ?? Guid.NewGuid().ToString(),
            authResponse.Data!.AccessToken,
            _options.Payments!.SigningKey,
            cancellationToken
        );
    }

    public async Task<ApiResponse<ListPaymentRefundsResponse>> ListPaymentRefunds(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        paymentId.NotNullOrWhiteSpace(nameof(paymentId));
        paymentId.NotAUrl(nameof(paymentId));

        var authResponse = await _auth.GetAuthToken(
            new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.GetAsync<ListPaymentRefundsResponse>(
            _baseUri.Append(paymentId).Append(PaymentsEndpoints.Refunds),
            authResponse.Data!.AccessToken,
            cancellationToken: cancellationToken
        );
    }

    public async Task<ApiResponse<RefundUnion>> GetPaymentRefund(
        string paymentId,
        string refundId,
        CancellationToken cancellationToken = default)
    {
        paymentId.NotNullOrWhiteSpace(nameof(paymentId));
        paymentId.NotAUrl(nameof(paymentId));
        refundId.NotNullOrWhiteSpace(nameof(refundId));
        refundId.NotAUrl(nameof(refundId));

        var authResponse = await _auth.GetAuthToken(
            new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.GetAsync<RefundUnion>(
            _baseUri.Append(paymentId).Append(PaymentsEndpoints.Refunds).Append(refundId),
            authResponse.Data!.AccessToken,
            cancellationToken: cancellationToken
        );
    }

    public async Task<ApiResponse> CancelPayment(
        string paymentId,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default)
    {
        paymentId.NotNullOrWhiteSpace(nameof(paymentId));

        paymentId.NotAUrl(nameof(paymentId));
        idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));

        var authResponse = await _auth.GetAuthToken(
            new GetAuthTokenRequest(AuthorizationScope.Payments), cancellationToken);

        if (!authResponse.IsSuccessful)
        {
            return new ApiResponse(authResponse.StatusCode, authResponse.TraceId);
        }

        return await _apiClient.PostAsync(
            _baseUri.Append(paymentId).Append(PaymentsEndpoints.Cancel),
            idempotencyKey: idempotencyKey ?? Guid.NewGuid().ToString(),
            accessToken: authResponse.Data!.AccessToken,
            signingKey: _options.Payments!.SigningKey,
            cancellationToken: cancellationToken);
    }
}