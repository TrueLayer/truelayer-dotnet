using System;

namespace TrueLayer.Payouts.Model
{
    public class InitiatePayoutRequest
    {
        public InitiatePayoutRequest(
            long amountInMinor,
            string currency,
            string beneficiaryName,
            string beneficiaryIban,
            string beneficiaryReference,
            string contextCode,
            Guid? transactionId = null)
        {
            AmountInMinor = amountInMinor;
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            BeneficiaryName = beneficiaryName.NotNullOrWhiteSpace(nameof(beneficiaryName));
            BeneficiaryIban = beneficiaryIban.NotNullOrWhiteSpace(nameof(beneficiaryIban));
            BeneficiaryReference = beneficiaryReference.NotNullOrWhiteSpace(nameof(beneficiaryReference));
            ContextCode = contextCode.NotNullOrWhiteSpace(nameof(contextCode));
            TransactionId = transactionId ?? Guid.NewGuid();
        }

        /// <summary>
        /// The unique id of the payout you want to initiate. If two payout requests are submitted with the same ID only the first one will be processed. 
        /// This can therefore be used to ensure idempotency for payout creation.
        /// </summary>
        /// <value></value>
        public Guid TransactionId { get; }

        /// <summary>
        /// The amount to send to the account holder in the smallest denomination of the currency specified,
        /// e.g. if transacting in GBP this will be in pennies.
        /// </summary>
        public long AmountInMinor { get; }

        /// <summary>
        /// The ISO 4217 currency code you are sending the payment in. 
        /// Each currency is associated with its own account, therefore the currency denotes which account the funds will be sent from.
        /// </summary>
        /// <value></value>
        public string Currency { get; }

        /// <summary>
        /// The name of the account holder you are sending funds to. This name must match the name associated with the account, otherwise the payout will fail validation. 
        /// It must be at most 18 characters long.
        /// </summary>
        public string BeneficiaryName { get; }

        /// <summary>
        /// The full IBAN of the account holder you are sending funds to. The IBAN must be accessible by the Faster Payments Scheme if transacting in GBP.
        /// </summary>
        public string BeneficiaryIban { get; }

        /// <summary>
        /// An 18 character reference that will appear on the account holder’s bank statement.
        /// </summary>
        public string BeneficiaryReference { get; }

        /// <summary>
        /// The code to describe why you are making the payout.
        /// </summary>
        public string ContextCode { get; }
    }
}
