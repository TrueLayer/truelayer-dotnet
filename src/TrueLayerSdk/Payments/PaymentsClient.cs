using System;
using System.Collections.Generic;
using System.Net.Http;
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
                    provider_id = "uk-cs-mock",
                    scheme_id = "sepa_credit_transfer",
                    fee_option_id = "free",
                    amount_in_minor = 120000,
                    currency = "GBP",
                    beneficiary = new Beneficiary
                    {
                        name = "A lucky someone",
                        account = new Account
                        {
                            account_number = "123456",
                            sort_code = "7890",
                            type = "sort_code",
                        },
                    },
                    remitter = new Remitter
                    {
                        name = "A less lucky someone",
                        account = new Account
                        {
                            account_number = "654321",
                            sort_code = "0987",
                            type = "sort_code",
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
            
            var apiResponse = await _apiClient.PostAsync<SingleImmediatePaymentInitiationResponse>(path, Functionality, cancellationToken, request.AccessToken, data);
            return apiResponse;
        }
    }
}
