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

        public PaymentsClient(IApiClient apiClient, TruelayerOptions options)
        {
            _apiClient = apiClient;
            
            BaseUri = options.Payments?.Uri ?? 
                       new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }
        
        public Task<GetPaymentStatusResponse> GetPaymentStatus(string paymentId, string accessToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(paymentId)) throw new ArgumentNullException(nameof(paymentId));
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));
            
            var path = $"single-immediate-payments/{paymentId}";
            return _apiClient.GetAsync<GetPaymentStatusResponse>(GetRequestUri(path), accessToken, cancellationToken);
        }
        
        public async Task<SingleImmediatePaymentResponse> SingleImmediatePayment(SingleImmediatePaymentRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.AccessToken)) throw new ArgumentNullException(nameof(request.AccessToken));

            const string path = "single-immediate-payments";

            return await _apiClient.PostAsync<SingleImmediatePaymentResponse>(GetRequestUri(path), request.Data, request.AccessToken, cancellationToken);
        }

        public async Task<SingleImmediatePaymentInitiationResponse> SingleImmediatePaymentInitiation(SingleImmediatePaymentInitiationRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.AccessToken)) throw new ArgumentNullException(nameof(request.AccessToken));

            const string path = "v2/single-immediate-payment-initiation-requests";

            return await _apiClient.PostAsync<SingleImmediatePaymentInitiationResponse>(GetRequestUri(path), request.Data, request.AccessToken, cancellationToken);
        }
        
        private Uri GetRequestUri(string path) => new (BaseUri, path);
    }
}
