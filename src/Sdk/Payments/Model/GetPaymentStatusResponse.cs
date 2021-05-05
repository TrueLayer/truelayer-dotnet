namespace TrueLayer.Payments.Model
{
    public record GetPaymentStatusResponse
    {
        public SingleImmediatePaymentResponse Result { get; set; } = null!;
    }
}
