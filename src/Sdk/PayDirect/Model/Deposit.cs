using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents the details of a deposit
    /// </summary>
    public record Deposit(
        Guid DepositId,
        DateTimeOffset InitiatedAt,
        string Status,
        string ProviderId,
        long AmountInMinor,
        string Currency,
        // Beneficiary
        // Remitter
        // References
        Deposit.SettlementDetails? Settled
    )
    {
        public record SettlementDetails(Guid AccountId, Guid TransactionId, DateTimeOffset SettledAt);
    }
}
