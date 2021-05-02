namespace TrueLayer.Payments.Model
{
    public class SingleImmediatePaymentRequest
    {
        public string AccessToken { get; set; }
        public SingleImmediatePaymentData Data { get; set; }
    }
}
