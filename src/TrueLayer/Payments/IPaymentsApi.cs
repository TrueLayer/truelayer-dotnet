using System;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.CreatePaymentResponse;
using OneOf;

namespace TrueLayer.Payments
{
    public interface IPaymentsApi
    {
        Task<ApiResponse<OneOf<AuthorizationRequired>>> CreatePayment(
            CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default);

        string CreateHostedPaymentPageLink(string paymentId, string resourceToken, Uri returnUri);
    }
}
