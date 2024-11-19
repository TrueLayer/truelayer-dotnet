using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Common;
using TrueLayer.Extensions;
using TrueLayer.Mandates.Model;
using TrueLayer.Models;

namespace TrueLayer.Mandates
{
    using AuthorizationResponseUnion = OneOf<
        AuthorisationFlowResponse.AuthorizationFlowAuthorizing,
        AuthorisationFlowResponse.AuthorizationFlowAuthorizationFailed>;
    using MandateDetailUnion = OneOf<
        MandateDetail.AuthorizationRequiredMandateDetail,
        MandateDetail.AuthorizingMandateDetail,
        MandateDetail.AuthorizedMandateDetail,
        MandateDetail.FailedMandateDetail,
        MandateDetail.RevokedMandateDetail>;

    internal class MandatesApi : IMandatesApi
    {
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

            var baseUri = (options.UseSandbox ?? true)
                ? TrueLayerBaseUris.SandboxApiBaseUri
                : TrueLayerBaseUris.ProdApiBaseUri;

            _baseUri = (options.Payments.Uri ?? baseUri)
                .Append("/mandates/");
        }

        /// <inheritdoc />
        public async Task<ApiResponse<CreateMandateResponse>> CreateMandate(CreateMandateRequest mandateRequest, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            mandateRequest.NotNull(nameof(mandateRequest));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));
            var type = mandateRequest.Mandate.Match(t0 => t0.Type, t1 => t1.Type);
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{type}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<CreateMandateResponse>(
                _baseUri,
                mandateRequest,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        //TODO: is it correct that this method expects a mandate type?
        /// <inheritdoc />
        public async Task<ApiResponse<MandateDetailUnion>> GetMandate(string mandateId, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            mandateId.NotNullOrWhiteSpace(nameof(mandateId));
            mandateId.NotAUrl(nameof(mandateId));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<MandateDetailUnion>(
                _baseUri.Append(mandateId),
                authResponse.Data!.AccessToken,
                cancellationToken: cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<ApiResponse<ResourceCollection<MandateDetailUnion>>> ListMandates(ListMandatesQuery query, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            var queryParameters = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryParameters["user_id"] = query.UserId.NotAUrl($"{nameof(query)}.{nameof(query.UserId)}");
            queryParameters["cursor"] = query.Cursor.NotAUrl($"{nameof(query)}.{nameof(query.UserId)}");
            queryParameters["limit"] = query.Limit.ToString();
            var baseUriBuilder = new UriBuilder(_baseUri) { Query = queryParameters.ToString() };

            return await _apiClient.GetAsync<ResourceCollection<MandateDetailUnion>>(
                baseUriBuilder.Uri,
                authResponse.Data!.AccessToken,
                cancellationToken: cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<ApiResponse<AuthorizationResponseUnion>> StartAuthorizationFlow(string mandateId, StartAuthorizationFlowRequest request, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            mandateId.NotNullOrWhiteSpace(nameof(mandateId));
            mandateId.NotAUrl(nameof(mandateId));
            request.NotNull(nameof(request));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<AuthorizationResponseUnion>(
                _baseUri.Append($"{mandateId}/authorization-flow"),
                request,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<ApiResponse<AuthorizationResponseUnion>> SubmitProviderSelection(string mandateId, SubmitProviderSelectionRequest request, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            mandateId.NotNullOrWhiteSpace(nameof(mandateId));
            mandateId.NotAUrl(nameof(mandateId));
            request.NotNull(nameof(request));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<AuthorizationResponseUnion>(
                _baseUri.Append($"{mandateId}/authorization-flow/actions/provider-selection"),
                request,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        public async Task<ApiResponse<AuthorizationResponseUnion>> SubmitConsent(string mandateId, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            mandateId.NotNullOrWhiteSpace(nameof(mandateId));
            mandateId.NotAUrl(nameof(mandateId));
            idempotencyKey.NotNullOrWhiteSpace(nameof(idempotencyKey));
            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync<AuthorizationResponseUnion>(
                _baseUri.Append($"{mandateId}/authorization-flow/actions/consent"),
                null,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<ApiResponse<GetConfirmationOfFundsResponse>> GetConfirmationOfFunds(string mandateId, int amountInMinor, string currency, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            mandateId.NotNullOrWhiteSpace(nameof(mandateId));
            mandateId.NotAUrl(nameof(mandateId));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<GetConfirmationOfFundsResponse>(
                _baseUri.Append($"{mandateId}/funds?amount_in_minor={amountInMinor}&currency={currency}"),
                authResponse.Data!.AccessToken,
                cancellationToken: cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<ApiResponse<GetConstraintsResponse>> GetMandateConstraints(string mandateId, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            mandateId.NotNullOrWhiteSpace(nameof(mandateId));
            mandateId.NotAUrl(nameof(mandateId));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.GetAsync<GetConstraintsResponse>(
                _baseUri.Append($"{mandateId}/constraints"),
                authResponse.Data!.AccessToken,
                cancellationToken: cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<ApiResponse> RevokeMandate(string mandateId, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default)
        {
            mandateId.NotNullOrWhiteSpace(nameof(mandateId));
            mandateId.NotAUrl(nameof(mandateId));

            ApiResponse<GetAuthTokenResponse> authResponse = await _auth.GetAuthToken(new GetAuthTokenRequest($"recurring_payments:{mandateType.AsString()}"), cancellationToken);

            if (!authResponse.IsSuccessful)
            {
                return new(authResponse.StatusCode, authResponse.TraceId);
            }

            return await _apiClient.PostAsync(
                _baseUri.Append($"{mandateId}/revoke"),
                null,
                idempotencyKey,
                authResponse.Data!.AccessToken,
                _options.Payments!.SigningKey,
                cancellationToken
            );
        }
    }
}
