using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents a request for a closed-loop withdrawal from a user
    /// </summary>
    public class UserWithdrawalRequest
    {
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

        public Guid UserId { get; }
        public Guid AccountId { get; }
        public Guid TransactionId { get; }
        public string BeneficiaryReference { get; }
        public long AmountInMinor { get; }
        public string Currency { get; }
    }
}
