using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Payments.Models;

namespace TrueLayerSdk.Payments
{
    public interface IPaymentsClient
    {
        Task<SingleImmediatePaymentInitiationResponse> SingleImmediatePaymentInitiation(
            SingleImmediatePaymentInitiationRequest request, CancellationToken cancellationToken);
    }
}
