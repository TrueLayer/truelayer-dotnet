namespace TrueLayer.Payments.Model
{
    public class Account
    {
        public string Type { get; set; }
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }
    }

    public class Beneficiary
    {
        public Account Account { get; set; }
        public string Name { get; set; }
    }

    public class Remitter
    {
        public Account Account { get; set; }
        public string Name { get; set; }
    }

    public class References
    {
        public string Type { get; set; }
        public string Beneficiary { get; set; }
        public string Remitter { get; set; }
    }

    public class SingleImmediatePayment
    {
        public string SingleImmediatePaymentId { get; set; }
        public string Status { get; set; }
        public string InitiatedAt { get; set; }
        public string ProviderId { get; set; }
        public string SchemeId { get; set; }
        public string FeeOptionId { get; set; }
        public int AmountInMinor { get; set; }
        public string Currency { get; set; }
        public Beneficiary Beneficiary { get; set; }
        public Remitter Remitter { get; set; }
        public References References { get; set; }
    }

    public class AuthFlow
    {
        public string Type { get; set; }
        public string ReturnUri { get; set; }
        public string Uri { get; set; }
        public string Expiry { get; set; }
    }

    public class SingleImmediatePaymentInitiationData
    {
        public SingleImmediatePayment SingleImmediatePayment { get; set; }
        public AuthFlow AuthFlow { get; set; }
    }
}
