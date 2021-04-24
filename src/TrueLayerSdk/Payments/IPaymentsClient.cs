using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Payments.Models;

namespace TrueLayerSdk.Payments
{
    public interface IPaymentsClient
    {
        /// <summary>
        /// v2
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SingleImmediatePaymentInitiationResponse> SingleImmediatePaymentInitiation(
            SingleImmediatePaymentInitiationRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// v1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SingleImmediatePaymentResponse> SingleImmediatePayment(SingleImmediatePaymentRequest request,
            CancellationToken cancellationToken);
    }
}
