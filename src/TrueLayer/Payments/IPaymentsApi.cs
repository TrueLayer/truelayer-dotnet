using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.GetPaymentResponse;

namespace TrueLayer.Payments
{
    // TODO Global Usings?
    using GetPaymentUnion = OneOf<
        AuthorizationRequired,
        Authorizing,
        Authorized,
        AuthorizationFailed,
        Successful,
        Settled,
        Failed
    >;
    
    public interface IPaymentsApi
    {
        Task<ApiResponse<OneOf<CreatePaymentResponse.AuthorizationRequired>>> CreatePayment(
            CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default);

        Task<ApiResponse<GetPaymentUnion>> GetPayment(string id, CancellationToken cancellationToken = default);

        string CreateHostedPaymentPageLink(string paymentId, string resourceToken, Uri returnUri);
    }
}
