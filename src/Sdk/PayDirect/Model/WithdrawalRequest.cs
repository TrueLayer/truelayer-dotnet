using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents a request for an open-loop withdrawal
    /// </summary>
    public class WithdrawalRequest
    {
        /// <summary>
        /// Request for a open-loop withdrawal 
        /// </summary>
        /// <param name="beneficiaryName">Name of the beneficiary you are sending funds to</param>
        /// <param name="beneficiaryIban">The full IBAN of the account holder you are sending funds to</param>
        /// <param name="beneficiaryReference">18 character reference that will appear on the account holder’s bank statement</param>
        /// <param name="amountInMinor">The amount in the minor currency unit</param>
        /// <param name="currency">The three-letter ISO alpha currency code</param>
        /// <param name="contextCode">The <see cref="ContextCodes">code</see> to describe why you are making the withdrawal</param>
        /// <param name="transactionId">The unique identifier of the transaction</param>
        public WithdrawalRequest(
            string beneficiaryName,
            string beneficiaryIban,
            string beneficiaryReference,
            long amountInMinor,
            string currency,
            string contextCode,
            Guid? transactionId = null
        )
        {
            BeneficiaryName = beneficiaryName.NotNullOrWhiteSpace(nameof(beneficiaryName));
            BeneficiaryIban = beneficiaryIban.NotNullOrWhiteSpace(nameof(beneficiaryIban));
            BeneficiaryReference = beneficiaryReference.NotNullOrWhiteSpace(nameof(beneficiaryReference));
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            ContextCode = contextCode.NotNullOrWhiteSpace(nameof(contextCode));
            TransactionId = transactionId ?? Guid.NewGuid();
        }

        /// <summary>
        /// Name of the beneficiary you are sending funds to
        /// </summary>
        public string BeneficiaryName { get; }
        
        /// <summary>
        /// The full IBAN of the account holder you are sending funds to
        /// </summary>
        public string BeneficiaryIban { get; }
        
        /// <summary>
        /// 18 character reference that will appear on the account holder’s bank statement
        /// </summary>
        public string BeneficiaryReference { get; }
        
        /// <summary>
        /// The amount in the minor currency unit
        /// </summary>
        public long AmountInMinor { get; }
        
        /// <summary>
        /// The three-letter ISO alpha currency code
        /// </summary>
        public string Currency { get; }
        
        /// <summary>
        /// The <see cref="ContextCodes">code</see> to describe why you are making the withdrawal
        /// </summary>
        public string ContextCode { get; }
        
        /// <summary>
        /// The unique identifier of the transaction
        /// </summary>
        public Guid TransactionId { get; }
    }
}
