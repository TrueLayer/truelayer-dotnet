using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

namespace TrueLayer.Payments
{
    public interface IPaymentsApi
    {
        Task<ApiResponse<CreatePaymentResponse>> CreatePayment(
            CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default);        
    }
}
