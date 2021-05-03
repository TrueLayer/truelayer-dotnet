using System;

namespace TrueLayer.Payments.Model
{

    public class InitiatePaymentRequest
    {
        public InitiatePaymentRequest(SingleImmediatePayment singleImmediatePayment, AuthFlow authFlow)
        {
            SingleImmediatePayment = singleImmediatePayment.NotNull(nameof(singleImmediatePayment));
            AuthFlow = authFlow.NotNull(nameof(authFlow));
        }

        public SingleImmediatePayment SingleImmediatePayment { get; }
        public AuthFlow AuthFlow { get; }
    }

    public class SingleImmediatePayment
    {
        public SingleImmediatePayment(long amountInMinor, string currency, string providerId, string schemeId, Beneficiary beneficiary, Guid? id = null)
        {
            AmountInMinor = amountInMinor;
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            ProviderId = providerId.NotNullOrWhiteSpace(nameof(providerId));
            SchemeId = schemeId.NotNullOrWhiteSpace(nameof(schemeId));
            Beneficiary = beneficiary.NotNull(nameof(beneficiary));
            SingleImmediatePaymentId = id ?? Guid.NewGuid();
        }

        /// <summary>
        /// The unique id of the payment you want to create. If two requests are received with the same id, only the first one will be processed.
        /// </summary>
        /// <value></value>
        public Guid SingleImmediatePaymentId { get; }

        /// <summary>
        /// The amount, specified in terms of the fractional monetary unit of the payment currency, to be paid.
        /// </summary>
        /// <value></value>
        public long AmountInMinor { get; }

        /// <summary>
        /// The ISO 4217 code of the currency for the payment.
        /// </summary>
        /// <value></value>
        public string Currency { get; }

        /// <summary>
        /// The id of the provider, as given on our /providers endpoint.
        /// </summary>
        /// <value></value>
        public string ProviderId { get; }

        /// <summary>
        /// The id of the scheme to make the payment over. This must be one of the schemes advertised by the provider from the provider results.
        /// </summary>
        /// <value></value>
        public string SchemeId { get; }

        /// <summary>
        /// The id indicating the desired distribution of fees between the beneficiary and remitter. 
        /// Must be a fee_option_id from one of the options advertised under that scheme on the /v2/single-immediate-payments-providers endpoint. 
        /// Should be omitted if there are no fee options (the scheme is free).
        /// </summary>
        /// <value></value>
        public string? FeeOptionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Beneficiary Beneficiary { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Remitter? Remitter { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public References? References { get; set; }
    }

    public class Beneficiary
    {
        public Beneficiary(Account account)
        {
            Account = account.NotNull(nameof(account));
        }
        
        public Account Account { get; }
        public string? Name { get; set; }
    }

    public class Remitter
    {
        public Remitter(Account account)
        {
            Account = account.NotNull(nameof(account));
        }
        
        public Account Account { get; }
        public string? Name { get; set; }
    }

    public class References
    {
        public string? Type { get; set; }
        public string? Beneficiary { get; set; }
        public string? Remitter { get; set; }
    }

    public class Account
    {
        public string? Type { get; set; }
        public string? SortCode { get; set; }
        public string? AccountNumber { get; set; }
    }

    public class AuthFlow
    {
        public AuthFlow(string type)
        {
            Type = type.NotNullOrWhiteSpace(nameof(type));
        }
        
        public string Type { get; set; }
        public string? ReturnUri { get; set; }
        public string? Uri { get; set; }
        public string? Expiry { get; set; }
    }
}
