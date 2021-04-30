using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Payments.Models;

namespace TrueLayerSdk.Payments
{
    /// <summary>
    /// Default implementation of <see cref="IPaymentsClient"/>.
    /// </summary>
    internal class PaymentsClient : IPaymentsClient
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

            var data = new SingleImmediatePaymentData
            {
                Amount = request.Amount,
                Currency = "GBP",
                RemitterProviderId = request.RemitterProviderId,
                RemitterName = request.RemitterName,
                RemitterSortCode = request.RemitterSortCode,
                RemitterAccountNumber = request.RemitterAccountNumber,
                RemitterReference = request.RemitterReference,
                BeneficiaryName = request.BeneficiaryName,
                BeneficiarySortCode = request.BeneficiarySortCode,
                BeneficiaryAccountNumber = request.BeneficiaryAccountNumber,
                BeneficiaryReference = request.BeneficiaryReference,
                RedirectUri = request.ReturnUri,
            };
            
            var apiResponse = await _apiClient.PostAsync<SingleImmediatePaymentResponse>(GetRequestUri(path), 
                data, request.AccessToken, cancellationToken);
            return apiResponse;
        }

        public async Task<SingleImmediatePaymentInitiationResponse> SingleImmediatePaymentInitiation(SingleImmediatePaymentInitiationRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.AccessToken)) throw new ArgumentNullException(nameof(request.AccessToken));

            const string path = "v2/single-immediate-payment-initiation-requests";

            var data = new SingleImmediatePaymentInitiationData
            {
                SingleImmediatePayment = new SingleImmediatePayment
                {
                    SingleImmediatePaymentId = Guid.NewGuid().ToString(),
                    ProviderId = "ob-sandbox-natwest",
                    SchemeId = "faster_payments_service",
                    FeeOptionId = "free",
                    AmountInMinor = 120000,
                    Currency = "GBP",
                    Beneficiary = new Beneficiary
                    {
                        Name = "A lucky someone",
                        Account = new Account
                        {
                            Type = "sort_code_account_number",
                            AccountNumber = "123456",
                            SortCode = "7890",
                        },
                    },
                    Remitter = new Remitter
                    {
                        Name = "A less lucky someone",
                        Account = new Account
                        {
                            Type = "sort_code_account_number",
                            AccountNumber = "654321",
                            SortCode = "0987",
                        },
                    },
                    References = new References
                    {
                        Type = "separate",
                        Beneficiary = "beneficiary ref",
                        Remitter = "remitter ref",
                    },
                },
                AuthFlow = new AuthFlow {Type = "redirect", ReturnUri = request.ReturnUri},
            };
            
            var apiResponse = await _apiClient.PostAsync<SingleImmediatePaymentInitiationData>(
                GetRequestUri(path), data, request.AccessToken, cancellationToken);
            
            return new SingleImmediatePaymentInitiationResponse {Data = apiResponse};
        }
        
        private Uri GetRequestUri(string path) => new (_configuration.PaymentsUri, path);
    }
}
