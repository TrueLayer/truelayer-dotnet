using System;

namespace TrueLayerSdk.SampleApp.Models
{
    public class PaymentData
    {
        public int Amount { get; set; }
        public string RemitterProviderId { get; set; }
        public string RemitterName { get; set; }
        public string RemitterSortCode { get; set; }
        public string RemitterAccountNumber { get; set; }
        public string RemitterReference { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiarySortCode { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string BeneficiaryReference { get; set; }

        public string SimpId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Status { get; set; }

    }
}
