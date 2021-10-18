using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.CreatePaymentResponse;

namespace TrueLayer.Payments
{
    public interface IPaymentsApi
    {
        Task<ApiResponse<Union<AuthorizationRequired>>> CreatePayment(
            CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default);        
    }
}
