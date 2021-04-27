using System;

namespace TrueLayerSdk.Payments.Models
{
    public class GetPaymentStatusData
    {
        public string SimpId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string BeneficiaryReference { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiarySortCode { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string RemitterReference { get; set; }
        public string RedirectUri { get; set; }
        public string RemitterProviderId { get; set; }
        public string Status { get; set; }
    }
}
