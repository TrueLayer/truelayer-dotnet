using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

namespace TrueLayerSdk.Payments
{
    /// <summary>
    /// Default implementation of <see cref="IPaymentsClient"/>.
    /// </summary>
    public class PaymentsClient : IPaymentsClient
    {
        private readonly IApiClient _apiClient;
        private readonly TruelayerConfiguration _configuration;

        public PaymentsClient(IApiClient apiClient, TruelayerConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
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

            return await _apiClient.PostAsync<SingleImmediatePaymentResponse>(GetRequestUri(path), cancellationToken, request.AccessToken, request.Data);
        }

        public async Task<SingleImmediatePaymentInitiationResponse> SingleImmediatePaymentInitiation(SingleImmediatePaymentInitiationRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.AccessToken)) throw new ArgumentNullException(nameof(request.AccessToken));

            const string path = "v2/single-immediate-payment-initiation-requests";

            return await _apiClient.PostAsync<SingleImmediatePaymentInitiationResponse>(GetRequestUri(path), cancellationToken, request.AccessToken, request.Data);
        }
        
        private Uri GetRequestUri(string path) => new (_configuration.PaymentsUri, path);
    }
}
