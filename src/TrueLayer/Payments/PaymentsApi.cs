using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

namespace TrueLayer.Payments
{
    internal class PaymentsApi : IPaymentsApi
    {
        internal const string ProdUrl = "https://api.truelayer.com/payments/";
        internal const string SandboxUrl = "https://api.truelayer-sandbox.com/payments/";
        internal static string[] RequiredScopes = new[] { "payments" };
        
        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        private readonly Uri _baseUri;

        public PaymentsApi(IApiClient apiClient, TrueLayerOptions options)
        {
            _apiClient = apiClient.NotNull(nameof(apiClient));
            _options = options.NotNull(nameof(options)); 

            options.Validate();

            _baseUri = options.Payments?.Uri ??
                       new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);

        }
        
        public Task<ApiResponse<CreatePaymentResponse>> CreatePayment(CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
