using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Common;
using TrueLayerSdk.Payments.Models;

namespace TrueLayerSdk.Payments
{
    /// <summary>
    /// Default implementation of <see cref="IPaymentsClient"/>.
    /// </summary>
    public class PaymentsClient : IPaymentsClient
    {
        private readonly IApiClient _apiClient;
        private readonly TruelayerConfiguration _configuration;
        private const Functionality Functionality = Common.Functionality.Payments;

        public PaymentsClient(IApiClient apiClient, TruelayerConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
        }

        public async Task<SingleImmediatePaymentResponse> SingleImmediatePayment(SingleImmediatePaymentRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.AccessToken)) throw new ArgumentNullException(nameof(request.AccessToken));

            const string path = "single-immediate-payments";

            var data = new SingleImmediatePaymentData
            {
                amount = 120000,
                currency = "GBP",
                remitter_provider_id = request.remitter_provider_id,
                remitter_name = request.remitter_name,
                remitter_sort_code = request.remitter_sort_code,
                remitter_account_number = request.remitter_account_number,
                remitter_reference = request.remitter_reference,
                beneficiary_name = request.beneficiary_name,
                beneficiary_sort_code = request.beneficiary_sort_code,
                beneficiary_account_number = request.beneficiary_account_number,
                beneficiary_reference = request.beneficiary_reference,
                redirect_uri = request.ReturnUri,
            };
            
            var apiResponse = await _apiClient.PostAsync<SingleImmediatePaymentResponse>(path, Functionality, cancellationToken, request.AccessToken, data);
            return apiResponse;
        }
        
        public async Task<SingleImmediatePaymentInitiationResponse> SingleImmediatePaymentInitiation(SingleImmediatePaymentInitiationRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.AccessToken)) throw new ArgumentNullException(nameof(request.AccessToken));

            const string path = "v2/single-immediate-payment-initiation-requests";

            var data = new SingleImmediatePaymentInitiationData
            {
                single_immediate_payment = new SingleImmediatePayment
                {
                    single_immediate_payment_id = Guid.NewGuid().ToString(),
                    provider_id = "ob-sandbox-natwest",
                    scheme_id = "faster_payments_service",
                    fee_option_id = "free",
                    amount_in_minor = 120000,
                    currency = "GBP",
                    beneficiary = new Beneficiary
                    {
                        name = "A lucky someone",
                        account = new Account
                        {
                            type = "sort_code_account_number",
                            account_number = "123456",
                            sort_code = "7890",
                        },
                    },
                    remitter = new Remitter
                    {
                        name = "A less lucky someone",
                        account = new Account
                        {
                            type = "sort_code_account_number",
                            account_number = "654321",
                            sort_code = "0987",
                        },
                    },
                    references = new References
                    {
                        type = "separate",
                        beneficiary = "beneficiary ref",
                        remitter = "remitter ref",
                    },
                },
                auth_flow = new AuthFlow {type = "redirect", return_uri = request.ReturnUri},
            };
            
            var apiResponse = await _apiClient.PostAsync<SingleImmediatePaymentInitiationData>(path, Functionality, cancellationToken, request.AccessToken, data);
            return new SingleImmediatePaymentInitiationResponse {Data = apiResponse};
        }
    }
}
