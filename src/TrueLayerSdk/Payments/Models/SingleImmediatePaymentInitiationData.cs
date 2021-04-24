namespace TrueLayerSdk.Payments.Models
{
    public class Account
    {
        public string type { get; set; }
        public string sort_code { get; set; }
        public string account_number { get; set; }
    }

    public class Beneficiary
    {
        public Account account { get; set; }
        public string name { get; set; }
    }

    public class Remitter
    {
        public Account account { get; set; }
        public string name { get; set; }
    }

    public class References
    {
        public string type { get; set; }
        public string beneficiary { get; set; }
        public string remitter { get; set; }
    }

    public class SingleImmediatePayment
    {
        public string single_immediate_payment_id { get; set; }
        public string status { get; set; }
        public string initiated_at { get; set; }
        public string provider_id { get; set; }
        public string scheme_id { get; set; }
        public string fee_option_id { get; set; }
        public int amount_in_minor { get; set; }
        public string currency { get; set; }
        public Beneficiary beneficiary { get; set; }
        public Remitter remitter { get; set; }
        public References references { get; set; }
    }

    public class AuthFlow
    {
        public string type { get; set; }
        public string return_uri { get; set; }
        public string uri { get; set; }
        public string expiry { get; set; }
    }

    public class SingleImmediatePaymentInitiationData
    {
        public SingleImmediatePayment single_immediate_payment { get; set; }
        public AuthFlow auth_flow { get; set; }
    }
}
