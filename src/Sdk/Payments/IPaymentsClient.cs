using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

namespace TrueLayer.Payments
{
    public interface IPaymentsClient
    {       
        /// <summary>
        /// v2
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<InitiatePaymentResponse> InitiatePayment(InitiatePaymentRequest request, string accessToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the status of the payment with the specified identifier string.
        /// </summary>
        /// <param name="paymentId">The payment or payment session identifier.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the payment details.</returns>
        Task<GetPaymentStatusResponse> GetPayment(string paymentId, string accessToken,
            CancellationToken cancellationToken = default);
    }
}
