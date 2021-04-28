namespace TrueLayer.Payments.Model
{
    public class SingleImmediatePaymentInitiationRequest
    {
        public string AccessToken { get; set; }
        public string ReturnUri { get; set; }
    }
}
