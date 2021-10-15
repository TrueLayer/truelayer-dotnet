using System;
using TrueLayer.Payments;

namespace TrueLayer
{
    internal class TrueLayerClient : ITrueLayerClient
    {
        private readonly Lazy<IPaymentsApi> _payments;
        private readonly Uri _baseUri;


        public TrueLayerClient(IApiClient apiClient, TrueLayerOptions options)
        {
            apiClient.NotNull(nameof(apiClient));
            options.NotNull(nameof(options));

            _payments = new(() => new PaymentsApi(apiClient, options));
        }

        public IPaymentsApi Payments => _payments.Value;
    }
}
