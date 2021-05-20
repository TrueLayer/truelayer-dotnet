using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;
using TrueLayer.Auth;

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
        private readonly IAuthClient _authClient;
        internal readonly Uri BaseUri;

        public PaymentsClient(IApiClient apiClient, TrueLayerOptions options, IAuthClient authClient)
        {
            _apiClient = apiClient;
            _authClient = authClient;

            BaseUri = options.Payments?.Uri ?? 
                      new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        public async Task<InitiatePaymentResponse> InitiatePayment(InitiatePaymentRequest request, CancellationToken cancellationToken)
        {
            request.NotNull(nameof(request));

            const string path = "v2/single-immediate-payment-initiation-requests";

            var accessToken = (await _authClient.GetPaymentToken(cancellationToken)).AccessToken;
            return await _apiClient.PostAsync<InitiatePaymentResponse>(GetRequestUri(path), request, accessToken, null, cancellationToken);
        }
        
        private Uri GetRequestUri(string path) => new (BaseUri, path);
    }
}
