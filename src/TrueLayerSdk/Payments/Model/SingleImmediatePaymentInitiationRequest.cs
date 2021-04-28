namespace TrueLayer.Payments.Model
{
    public class SingleImmediatePaymentInitiationRequest
    {
        public string AccessToken { get; set; }
        public SingleImmediatePaymentInitiationData Data { get; set; }
    }
}
