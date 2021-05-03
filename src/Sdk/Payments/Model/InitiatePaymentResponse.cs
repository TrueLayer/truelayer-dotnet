using System;

namespace TrueLayer.Payments.Model
{
    public record InitiatePaymentResponse
    {
        public InitiatePaymentResponseResult Result { get; init; } = null!;
    }

    public record InitiatePaymentResponseResult
    {
        public SingleImmediatePaymentResponse SingleImmediatePayment { get; init; } = null!;
        public AuthFlowResponse AuthFlow { get; init; } = null!;
    }

    public record SingleImmediatePaymentResponse
    {
        public Guid SingleImmediatePaymentId { get; init; }
        public DateTimeOffset InitiatedAt { get; init; }
        public string Status { get; init; } = null!;
    }

    public record AuthFlowResponse
    {

    }
}
