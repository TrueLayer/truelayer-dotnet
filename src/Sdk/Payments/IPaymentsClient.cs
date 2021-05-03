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
    }
}
