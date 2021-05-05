using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

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

        public Task<GetPaymentStatusResponse> GetPayment(string paymentId, string accessToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(paymentId)) throw new ArgumentNullException(nameof(paymentId));
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));
            
            var path = $"v2/single-immediate-payments/{paymentId}";
            return _apiClient.GetAsync<GetPaymentStatusResponse>(GetRequestUri(path), accessToken, cancellationToken);
        }

        private Uri GetRequestUri(string path) => new (BaseUri, path);
    }
}
