namespace TrueLayerSdk.Payments.Models
{
    public class SingleImmediatePaymentData
    {
        public string SimpId { get; set; }
        public string AuthUri { get; set; }
        public string CreatedAt { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string RemitterProviderId { get; set; }
        public string RemitterName { get; set; }
        public string RemitterSortCode { get; set; }
        public string RemitterAccountNumber { get; set; }
        public string RemitterReference { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiarySortCode { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string BeneficiaryReference { get; set; }
        public string RedirectUri { get; set; }
        public string Status { get; set; }
    }
}
