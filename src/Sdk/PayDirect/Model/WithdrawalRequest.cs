using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents a request for an open-loop withdrawal
    /// </summary>
    public class WithdrawalRequest
    {
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

        public string BeneficiaryName { get; }
        public string BeneficiaryIban { get; }
        public string BeneficiaryReference { get; }
        public long AmountInMinor { get; }
        public string Currency { get; }
        public string ContextCode { get; }
        public Guid TransactionId { get; }
    }
}
