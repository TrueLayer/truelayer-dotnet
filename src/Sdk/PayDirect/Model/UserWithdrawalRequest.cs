using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents a request for a closed-loop withdrawal to a user
    /// </summary>
    public class UserWithdrawalRequest
    {
        /// <summary>
        /// Request for a closed-loop withdrawal 
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="accountId">The user account identifier</param>
        /// <param name="beneficiaryReference">18 character reference that will appear on the account holder’s bank statement</param>
        /// <param name="amountInMinor">The amount in the minor currency unit</param>
        /// <param name="currency">The three-letter ISO alpha currency code</param>
        /// <param name="transactionId">The unique identifier of the transaction</param>
        public UserWithdrawalRequest(
            Guid userId,
            Guid accountId,
            string beneficiaryReference,
            long amountInMinor,
            string currency,
            Guid? transactionId = null
        )
        {
            UserId = userId;
            AccountId = accountId;
            BeneficiaryReference = beneficiaryReference.NotNullOrWhiteSpace(nameof(beneficiaryReference));
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            TransactionId = transactionId ?? Guid.NewGuid();
        }

        /// <summary>
        /// The user identifier
        /// </summary>
        public Guid UserId { get; }
        
        /// <summary>
        /// The user account identifier
        /// </summary>
        public Guid AccountId { get; }
        
        /// <summary>
        /// The unique identifier of the transaction
        /// </summary>
        public Guid TransactionId { get; }
        
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
    }
}
