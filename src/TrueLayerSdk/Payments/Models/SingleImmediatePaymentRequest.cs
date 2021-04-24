namespace TrueLayerSdk.Payments.Models
{
    public class SingleImmediatePaymentRequest
    {
        public string AccessToken { get; set; }
        public string ReturnUri { get; set; }
        
        public int amount { get; set; }
        public string remitter_provider_id { get; set; }
        public string remitter_name { get; set; }
        public string remitter_sort_code { get; set; }
        public string remitter_account_number { get; set; }
        public string remitter_reference { get; set; }
        public string beneficiary_name { get; set; }
        public string beneficiary_sort_code { get; set; }
        public string beneficiary_account_number { get; set; }
        public string beneficiary_reference { get; set; }
    }
}
