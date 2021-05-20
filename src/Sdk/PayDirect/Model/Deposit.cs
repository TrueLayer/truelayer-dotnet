using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents an existing deposit
    /// </summary>
    /// <param name="DepositId">The deposit identifier</param>
    /// <param name="InitiatedAt">The date/time the deposit was initiated</param>
    /// <param name="Status">The status of the deposit</param>
    /// <param name="ProviderId">The provider used as the source of funds</param>
    /// <param name="AmountInMinor">The amount of the deposit in the minor currency unit</param>
    /// <param name="Currency">The three-letter ISO alpha code of the currency</param>
    /// <param name="Settled">The settlement details, if the deposit was settled</param>
    /// <returns></returns>
    public record Deposit(
        Guid DepositId,
        DateTimeOffset InitiatedAt,
        string Status,
        string ProviderId,
        long AmountInMinor,
        string Currency,
        Deposit.SettlementDetails? Settled
    )
    {
        /// <summary>
        /// Gets whether the deposit has been settled
        /// </summary>
        public bool IsSettled => Status == DepositStatuses.Settled;
        
        /// <summary>
        /// Deposit settlement details
        /// </summary>
        /// <param name="AccountId">The identifier of the user account created as a result of the settled deposit</param>
        /// <param name="TransactionId">The transaction identifier</param>
        /// <param name="SettledAt">The date/time the deposit was settled</param>
        /// <returns></returns>
        public record SettlementDetails(Guid AccountId, Guid TransactionId, DateTimeOffset SettledAt);
    }
}
