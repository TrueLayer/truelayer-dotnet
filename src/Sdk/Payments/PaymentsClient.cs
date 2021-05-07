using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;
using System.Linq;
using System.Web;

namespace TrueLayer.Payments
{
    /// <summary>
    /// Default implementation of <see cref="IPaymentsClient"/>.
    /// </summary>
    internal class PaymentsClient : IPaymentsClient
    {
        internal const string ProdUrl = "https://pay-api.truelayer.com/";
        internal const string SandboxUrl = "https://pay-api.truelayer-sandbox.com/";

        private readonly IApiClient _apiClient;
        internal readonly Uri BaseUri;

        public PaymentsClient(IApiClient apiClient, TrueLayerOptions options)
        {
            _apiClient = apiClient;
            
            BaseUri = options.Payments?.Uri ?? 
                       new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        public async Task<InitiatePaymentResponse> InitiatePayment(InitiatePaymentRequest request, string accessToken, CancellationToken cancellationToken)
        {
            request.NotNull(nameof(request));
            accessToken.NotNullOrWhiteSpace(nameof(accessToken));

            const string path = "v2/single-immediate-payment-initiation-requests";

            return await _apiClient.PostAsync<InitiatePaymentResponse>(GetRequestUri(path), request, accessToken, cancellationToken);
        }

        public async Task<GetPaymentStatusResponse> GetPayment(string paymentId, string accessToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(paymentId)) throw new ArgumentNullException(nameof(paymentId));
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));
            
            var path = $"v2/single-immediate-payments/{paymentId}";
            return await _apiClient.GetAsync<GetPaymentStatusResponse>(GetRequestUri(path), accessToken, cancellationToken);
        }

        public async Task<GetProvidersResponse> GetProviders(GetProvidersRequest request, CancellationToken cancellationToken = default)
        {
            const string path = "v2/single-immediate-payments-providers";

            Uri BuildProvidersUriWithParams()
            {
                var requestUri = GetRequestUri(path);
            
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["client_id"] = request.ClientId;
                query["auth_flow_type"] = request.AuthFlowType.Aggregate((a,b) => $"{a},{b}");
                query["account_type"] = request.AccountType.Aggregate((a,b) => $"{a},{b}");;
                query["currency"] = request.Currency.Aggregate((a,b) => $"{a},{b}");
                if (request.Country is { }) query["country"] = request.Country?.Aggregate((a,b) => $"{a},{b}");
                if (request.AdditionalInputType is { }) query["additional_input_type"] = request.AdditionalInputType?.Aggregate((a,b) => $"{a},{b}");
                if (request.ReleaseChannel is { }) query["release_channel"] = request.ReleaseChannel;
            
                var builder = new UriBuilder(requestUri) {Query = query.ToString()};
                return builder.Uri;
            };

            return await _apiClient.GetAsync<GetProvidersResponse>(BuildProvidersUriWithParams(), string.Empty, cancellationToken);
        }

        private Uri GetRequestUri(string path) => new (BaseUri, path);
    }
}
